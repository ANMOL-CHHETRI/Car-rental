using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace RentOnCars.Services
{
    public sealed class StaffIdentity
    {
        public string Username { get; set; }
        public string Role { get; set; }
    }

    public static class StaffSession
    {
        private static StaffIdentity identity;
        private static DateTime expiresAt;
        public static StaffIdentity Current => DateTime.UtcNow < expiresAt ? identity : null;
        public static void SignIn(StaffIdentity staff) { identity = staff; expiresAt = DateTime.UtcNow.AddMinutes(20); }
        public static void Touch() { if (Current != null) expiresAt = DateTime.UtcNow.AddMinutes(20); }
        public static void SignOut() { identity = null; expiresAt = DateTime.MinValue; }
        public static string RequireUsername()
        {
            if (Current == null) throw new UnauthorizedAccessException("Sign in again to continue.");
            Touch();
            return identity.Username;
        }
    }

    public static class StaffAuthentication
    {
        private const int WorkFactor = 600000;
        public static void EnsureInitialAccount(SqlConnection connection)
        {
            string username = Environment.GetEnvironmentVariable("WEARECARS_INITIAL_USER");
            string password = Environment.GetEnvironmentVariable("WEARECARS_INITIAL_PASSWORD");
            bool demo=false;
            if (Environment.GetEnvironmentVariable("WEARECARS_DEMO") == "1" && string.IsNullOrEmpty(username))
            {
                CreateAccount(connection, "sta001", "givemethekeys123", "admin", true);
                CreateAccount(connection, "admin", "admin", "admin", true);
            }
            else if (!string.IsNullOrEmpty(username))
            {
                CreateAccount(connection, username, password, Environment.GetEnvironmentVariable("WEARECARS_INITIAL_ROLE") ?? "admin", demo);
            }
            using(var cmd=new SqlCommand("IF NOT EXISTS(SELECT 1 FROM dbo.StaffAccounts WHERE Username='self-service') INSERT dbo.StaffAccounts(Username,PasswordHash,Salt,Iterations,Role,IsActive,IsDemo) VALUES('self-service',0x00,0x00,600000,'staff',1,0);", connection))
            { cmd.ExecuteNonQuery(); }
        }

        public static void CreateAccount(SqlConnection connection, string username, string password, string role, bool demo=false)
        {
            if (!Regex.IsMatch(username ?? "", @"^[A-Za-z0-9._-]{3,50}$")) throw new ArgumentException("Username must be 3–50 letters, numbers, dots, hyphens or underscores.");
            if (string.IsNullOrEmpty(password) || password.Length < 4) throw new ArgumentException("Use a password of at least 4 characters.");
            if (role != "staff" && role != "admin") throw new ArgumentException("Invalid staff role.");
            byte[] salt = new byte[32]; using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);
            byte[] hash; using (var kdf = new Rfc2898DeriveBytes(password, salt, WorkFactor, HashAlgorithmName.SHA256)) hash = kdf.GetBytes(32);
            using (var cmd = new SqlCommand("IF NOT EXISTS(SELECT 1 FROM dbo.StaffAccounts WHERE Username=@u) INSERT dbo.StaffAccounts(Username,PasswordHash,Salt,Iterations,Role,IsDemo) VALUES(@u,@h,@s,@i,@r,@demo); ELSE IF @demo=1 UPDATE dbo.StaffAccounts SET PasswordHash=@h,Salt=@s,Iterations=@i,Role=@r,IsActive=1,IsDemo=1,FailedAttempts=0,LockedUntil=NULL WHERE Username=@u;", connection))
            {
                cmd.Parameters.Add("@u", SqlDbType.NVarChar, 50).Value = username;
                cmd.Parameters.Add("@h", SqlDbType.VarBinary, 32).Value = hash;
                cmd.Parameters.Add("@s", SqlDbType.VarBinary, 32).Value = salt;
                cmd.Parameters.Add("@i", SqlDbType.Int).Value = WorkFactor;
                cmd.Parameters.Add("@r", SqlDbType.NVarChar, 10).Value = role;
                cmd.Parameters.Add("@demo", SqlDbType.Bit).Value = demo;
                cmd.ExecuteNonQuery();
            }
        }

        public static StaffIdentity Authenticate(string connectionString, string username, string password)
        {
            if (!Regex.IsMatch(username ?? "", @"^[A-Za-z0-9._-]{3,50}$") || string.IsNullOrEmpty(password) || password.Length > 256) return null;
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                using (var transaction = con.BeginTransaction())
                {
                    byte[] hash = null, salt = null; int iterations = WorkFactor; string role = null; int failures = 0; bool locked = false;
                    using (var cmd = new SqlCommand("SELECT PasswordHash,Salt,Iterations,Role,FailedAttempts,LockedUntil,IsActive,IsDemo FROM dbo.StaffAccounts WITH(UPDLOCK,HOLDLOCK) WHERE Username=@u", con, transaction))
                    {
                        cmd.Parameters.Add("@u", SqlDbType.NVarChar, 50).Value = username;
                        using (var rd = cmd.ExecuteReader()) if (rd.Read())
                        {
                            hash = (byte[])rd[0]; salt = (byte[])rd[1]; iterations = rd.GetInt32(2); role = rd.GetString(3); failures = rd.GetInt32(4);
                            locked = !rd.GetBoolean(6) || (rd.GetBoolean(7) && Environment.GetEnvironmentVariable("WEARECARS_DEMO")!="1") || (!rd.IsDBNull(5) && rd.GetDateTime(5) > DateTime.UtcNow);
                        }
                    }
                    bool valid = false;
                    if (hash != null && !locked)
                    {
                        byte[] candidate; using (var kdf = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256)) candidate = kdf.GetBytes(32);
                        int difference = hash.Length ^ candidate.Length;
                        for (int i = 0; i < Math.Min(hash.Length, candidate.Length); i++) difference |= hash[i] ^ candidate[i];
                        valid = difference == 0;
                    }
                    else if (hash == null)
                    { using (var kdf = new Rfc2898DeriveBytes(password, new byte[32], WorkFactor, HashAlgorithmName.SHA256)) kdf.GetBytes(32); }
                    if (hash != null && !locked)
                    using (var cmd = new SqlCommand(valid ? "UPDATE dbo.StaffAccounts SET FailedAttempts=0,LockedUntil=NULL WHERE Username=@u" : "UPDATE dbo.StaffAccounts SET FailedAttempts=@n,LockedUntil=CASE WHEN @n>=5 THEN DATEADD(minute,10,SYSUTCDATETIME()) ELSE NULL END WHERE Username=@u", con, transaction))
                    {
                        cmd.Parameters.Add("@u", SqlDbType.NVarChar, 50).Value = username;
                        if (!valid) cmd.Parameters.Add("@n", SqlDbType.Int).Value = failures + 1;
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = new SqlCommand("INSERT dbo.AuditEvents(Actor,Action,Detail) VALUES(@u,@a,'Staff sign-in')", con, transaction))
                    {
                        cmd.Parameters.Add("@u", SqlDbType.NVarChar, 50).Value = username;
                        cmd.Parameters.Add("@a", SqlDbType.NVarChar, 30).Value = valid ? "LoginSuccess" : "LoginFailure";
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    return valid ? new StaffIdentity { Username = username, Role = role } : null;
                }
            }
        }
    }
}
