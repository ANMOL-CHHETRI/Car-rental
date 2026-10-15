using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RentOnCars.Services;

namespace RentOnCars
{
    /// <summary>
    /// Status information resulting from a database preparation attempt.
    /// </summary>
    public class DatabaseInitResult
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Guidance { get; set; } = string.Empty;
        public Exception Error { get; set; }
    }

    /// <summary>
    /// Automatically and non-destructively prepares the SQL Server / LocalDB database.
    /// Preserves existing records, starter cars, and user bookings without dropping tables.
    /// </summary>
    public static class DatabaseInitializer
    {
        private const string DatabaseName = "RentOnCarsDB";
        private const string LocalDbInstance = "MSSQLLocalDB";

        public static bool IsInitialized { get; private set; }
        public static DatabaseInitResult LastResult { get; private set; } = new DatabaseInitResult();

        public static string ConnectionString
        {
            get
            {
                string configured = Environment.GetEnvironmentVariable("WEARECARS_CONNECTION_STRING");
                if (!string.IsNullOrWhiteSpace(configured)) return configured;
                ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["RentOnCarsDb"];
                if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
                    return @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=RentOnCarsDB;Integrated Security=True;Connect Timeout=5";

                return setting.ConnectionString;
            }
        }

        /// <summary>
        /// Attempts to initialize the database and returns a detailed status result.
        /// Does not throw exceptions, ensuring the application startup remains reliable.
        /// </summary>
        public static DatabaseInitResult Initialize(string customConnectionString = null)
        {
            DatabaseInitResult result = new DatabaseInitResult();
            try
            {
                string configured = customConnectionString ?? ConnectionString;
                var builder = new SqlConnectionStringBuilder(configured) { ConnectTimeout = 5 };
                if (string.IsNullOrWhiteSpace(builder.InitialCatalog))
                    throw new InvalidOperationException("A target database catalog must be configured.");
                if (Environment.GetEnvironmentVariable("WEARECARS_SKIP_INITIALIZATION")=="1")
                {
                    using(var con=new SqlConnection(builder.ConnectionString)) {con.Open();using(var cmd=new SqlCommand("SELECT MAX(Version) FROM dbo.SchemaVersions",con))if(Convert.ToInt32(cmd.ExecuteScalar())!=2)throw new InvalidOperationException("Prepared schema version 2 is required.");}
                    IsInitialized=result.IsSuccessful=true;result.Message="Prepared database connected";LastResult=result;return result;
                }
                if (builder.DataSource.StartsWith("(localdb)\\", StringComparison.OrdinalIgnoreCase))
                    EnsureLocalDbInstance(builder.DataSource.Substring(10));
                EnsureDatabaseExists(builder);
                using (var connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("DECLARE @r int; EXEC @r=sys.sp_getapplock @Resource='WeAreCarsSchema', @LockMode='Exclusive', @LockOwner='Session', @LockTimeout=15000; IF @r<0 THROW 50003, 'Schema is being prepared by another process.', 1;", connection))
                        command.ExecuteNonQuery();
                    ApplyMigrations(connection);
                    StaffAuthentication.EnsureInitialAccount(connection);
                }
                IsInitialized = result.IsSuccessful = true;
                result.Message = "Database ready";
                result.Guidance = builder.InitialCatalog + " on " + builder.DataSource;
            }
            catch (Exception ex)
            {
                IsInitialized = false;
                result.Error = ex;
                result.Message = "Database preparation failed: " + ex.Message;
                result.Guidance = "Check the configured server/catalog and schema migration errors. " + GenerateGuidanceMessage(ex);
            }
            LastResult = result;
            return result;
        }

        private static string GenerateGuidanceMessage(Exception ex)
        {
            string sqlLocalDb = FindSqlLocalDbExecutable();
            if (string.IsNullOrEmpty(sqlLocalDb))
            {
                return "SQL Server LocalDB executable (SqlLocalDB.exe) was not found on this computer.\n\n" +
                       "Action required to enable database saving & reports:\n" +
                       "1. Open Visual Studio Installer -> Modify -> Workloads.\n" +
                       "2. Under '.NET desktop development', ensure 'SQL Server Express 2019/2022 LocalDB' is checked and installed.\n" +
                       "3. Alternatively, install SQL Server Express or configure your connection string in App.config to point to your existing SQL instance.\n\n" +
                       "Offline estimate calculations and input validation will continue to function.";
            }

            return "SqlLocalDB.exe was detected but the database instance could not be contacted.\n\n" +
                   $"Technical details: {ex.Message}\n\n" +
                   "Troubleshooting steps:\n" +
                   "1. Open a command prompt and run: sqllocaldb start MSSQLLocalDB\n" +
                   "2. Verify the instance is running with: sqllocaldb info MSSQLLocalDB\n" +
                   "3. Click 'Retry Database Connection' on the Dashboard.";
        }

        private static void EnsureLocalDbInstance(string instance)
        {
            if (!Regex.IsMatch(instance, @"^[a-zA-Z0-9_-]+$")) throw new ArgumentException("Invalid LocalDB instance name.");
            string executable = FindSqlLocalDbExecutable();
            if (string.IsNullOrEmpty(executable)) return;
            string info = RunSqlLocalDb(executable, "info " + instance);
            if (info.IndexOf("not created", StringComparison.OrdinalIgnoreCase) >= 0 ||
                info.IndexOf("does not exist", StringComparison.OrdinalIgnoreCase) >= 0)
                RunSqlLocalDb(executable, "create " + instance);
            RunSqlLocalDb(executable, "start " + instance);
        }

        /// <summary>
        /// Locates SqlLocalDB.exe by inspecting PATH first, followed by standard versioned SQL Server paths.
        /// Resolves the bug where bare executable name was returned before validating availability.
        /// </summary>
        public static string FindSqlLocalDbExecutable()
        {
            // 1. Check if sqllocaldb is accessible on the current PATH
            string pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            string[] pathDirs = pathEnv.Split(new[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string dir in pathDirs)
            {
                try
                {
                    string candidate = Path.Combine(dir.Trim(), "SqlLocalDB.exe");
                    if (File.Exists(candidate))
                        return candidate;
                }
                catch
                {
                    // Ignore invalid path entries in PATH environment variable
                }
            }

            // 2. Check standard versioned installation directories for SQL Server 2025, 2022, 2019, 2017, 2016, 2014, 2012
            string[] versions = { "170", "160", "150", "140", "130", "120", "110" };
            string progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string progFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            foreach (string v in versions)
            {
                string p64 = Path.Combine(progFiles, "Microsoft SQL Server", v, "Tools", "Binn", "SqlLocalDB.exe");
                if (File.Exists(p64))
                    return p64;

                if (!string.IsNullOrEmpty(progFilesX86))
                {
                    string p32 = Path.Combine(progFilesX86, "Microsoft SQL Server", v, "Tools", "Binn", "SqlLocalDB.exe");
                    if (File.Exists(p32))
                        return p32;
                }
            }

            return null;
        }

        private static string RunSqlLocalDb(string executable, string arguments)
        {
            var psi = new ProcessStartInfo { FileName = executable, Arguments = arguments,
                UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
            using (Process process = Process.Start(psi))
            {
                if (process == null) throw new InvalidOperationException("Unable to start LocalDB tools.");
                Task<string> output = process.StandardOutput.ReadToEndAsync();
                Task<string> error = process.StandardError.ReadToEndAsync();
                if (!process.WaitForExit(15000))
                {
                    try { process.Kill(); } catch (InvalidOperationException) { }
                    throw new TimeoutException("LocalDB tools exceeded the 15-second deadline.");
                }
                Task.WaitAll(output, error);
                string message = output.Result + Environment.NewLine + error.Result;
                if (process.ExitCode != 0 && !arguments.StartsWith("info ", StringComparison.Ordinal))
                    throw new InvalidOperationException(message.Trim());
                return message;
            }
        }

        private static void EnsureDatabaseExists(SqlConnectionStringBuilder configured)
        {
            string catalog = configured.InitialCatalog;
            var master = new SqlConnectionStringBuilder(configured.ConnectionString) { InitialCatalog = "master" };
            using (var connection = new SqlConnection(master.ConnectionString))
            {
                connection.Open();
                string escaped = catalog.Replace("]", "]]");
                using (var command = new SqlCommand("IF DB_ID(@catalog) IS NULL CREATE DATABASE [" + escaped + "];", connection))
                {
                    command.Parameters.Add("@catalog", SqlDbType.NVarChar, 128).Value = catalog;
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void ApplyMigrations(SqlConnection connection)
        {
            using (var version = new SqlCommand("IF OBJECT_ID('dbo.SchemaVersions') IS NOT NULL SELECT ISNULL(MAX(Version),0) FROM dbo.SchemaVersions ELSE SELECT 0", connection))
                if (Convert.ToInt32(version.ExecuteScalar()) > 2) throw new InvalidOperationException("This application cannot open a newer database schema.");
            string sql;
            using (Stream stream = typeof(DatabaseInitializer).Assembly.GetManifestResourceStream("RentOnCars.DatabaseSchema.sql"))
            {
                if (stream == null) throw new InvalidOperationException("Canonical database schema resource is missing.");
                using (var reader = new StreamReader(stream)) sql = reader.ReadToEnd();
            }
            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    foreach (string batch in Regex.Split(sql, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
                        if (!string.IsNullOrWhiteSpace(batch))
                            using (var command = new SqlCommand(batch, connection, transaction)) { command.CommandTimeout = 30; command.ExecuteNonQuery(); }
                    transaction.Commit();
                }
                catch { try { transaction.Rollback(); } catch (InvalidOperationException) { } throw; }
            }
        }
    }
}
