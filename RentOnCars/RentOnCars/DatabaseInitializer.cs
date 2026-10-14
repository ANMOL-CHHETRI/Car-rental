using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

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
                ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["RentOnCarsDb"];
                if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
                    throw new InvalidOperationException("The 'RentOnCarsDb' connection string is missing from App.config.");

                return setting.ConnectionString;
            }
        }

        /// <summary>
        /// Attempts to initialize the database and returns a detailed status result.
        /// Does not throw exceptions, ensuring the application startup remains reliable.
        /// </summary>
        public static DatabaseInitResult Initialize()
        {
            DatabaseInitResult result = new DatabaseInitResult();

            try
            {
                // Step 1: Ensure LocalDB instance is created and running if LocalDB is installed
                EnsureLocalDbInstance();

                // Step 2: Ensure database catalog exists on SQL Server
                EnsureDatabaseExists();

                // Step 3: Ensure tables exist with proper constraints and starter fleet is seeded
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    CreateTables(connection);
                    SeedStarterFleet(connection);
                }

                IsInitialized = true;
                result.IsSuccessful = true;
                result.Message = "Database is online and ready.";
                result.Guidance = "Connected to SQL Server / LocalDB (RentOnCarsDB).";
            }
            catch (Exception ex)
            {
                IsInitialized = false;
                result.IsSuccessful = false;
                result.Error = ex;
                result.Message = "Database connection could not be established.";
                result.Guidance = GenerateGuidanceMessage(ex);
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

        private static void EnsureLocalDbInstance()
        {
            string sqlLocalDb = FindSqlLocalDbExecutable();
            if (string.IsNullOrEmpty(sqlLocalDb))
                return;

            string info = RunSqlLocalDb(sqlLocalDb, $"info {LocalDbInstance}");

            if (string.IsNullOrWhiteSpace(info) ||
                info.IndexOf("does not exist", StringComparison.OrdinalIgnoreCase) >= 0 ||
                info.IndexOf("LocalDB instance is not running", StringComparison.OrdinalIgnoreCase) >= 0 ||
                info.IndexOf("State: Stopped", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                // Create if not exists, then start
                RunSqlLocalDb(sqlLocalDb, $"create {LocalDbInstance}");
                RunSqlLocalDb(sqlLocalDb, $"start {LocalDbInstance}");
            }
            else if (info.IndexOf("State: Running", StringComparison.OrdinalIgnoreCase) < 0)
            {
                RunSqlLocalDb(sqlLocalDb, $"start {LocalDbInstance}");
            }
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
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = executable,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process process = Process.Start(psi))
                {
                    if (process == null)
                        return string.Empty;

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit(15000);

                    return output + Environment.NewLine + error;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DatabaseInitializer] RunSqlLocalDb failed: {ex.Message}");
                return string.Empty;
            }
        }

        private static void EnsureDatabaseExists()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString)
            {
                InitialCatalog = "master",
                ConnectTimeout = 10
            };

            using (SqlConnection masterConn = new SqlConnection(builder.ConnectionString))
            {
                masterConn.Open();

                using (SqlCommand cmd = new SqlCommand(
                    "IF DB_ID(@DatabaseName) IS NULL CREATE DATABASE [RentOnCarsDB];", masterConn))
                {
                    cmd.CommandTimeout = 30;
                    cmd.Parameters.Add("@DatabaseName", SqlDbType.NVarChar, 128).Value = DatabaseName;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void CreateTables(SqlConnection connection)
        {
            // Create Cars table if missing
            ExecuteNonQuery(connection, @"
IF OBJECT_ID(N'dbo.Cars', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Cars
    (
        CarId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Cars PRIMARY KEY,
        RegistrationNumber NVARCHAR(20) NOT NULL CONSTRAINT UQ_Cars_Registration UNIQUE,
        IsRented BIT NOT NULL CONSTRAINT DF_Cars_IsRented DEFAULT (0),
        DailyRate DECIMAL(10,2) NOT NULL CONSTRAINT CK_Cars_DailyRate CHECK (DailyRate > 0),
        CarType NVARCHAR(30) NOT NULL CONSTRAINT CK_Cars_CarType CHECK
            (CarType IN (N'CityCar', N'FamilyCar', N'SportsCar', N'SUV')),
        FuelType NVARCHAR(30) NOT NULL CONSTRAINT CK_Cars_FuelType CHECK
            (FuelType IN (N'Petrol', N'Diesel', N'Hybrid', N'FullElectric'))
    );
END");

            // Create Bookings table if missing
            ExecuteNonQuery(connection, @"
IF OBJECT_ID(N'dbo.Bookings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Bookings
    (
        BookingId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Bookings PRIMARY KEY,
        CarId INT NOT NULL,
        CustomerFirstName NVARCHAR(50) NOT NULL,
        CustomerSurname NVARCHAR(50) NOT NULL,
        CustomerAddress NVARCHAR(150) NOT NULL,
        CustomerAge INT NOT NULL CONSTRAINT CK_Bookings_Age CHECK (CustomerAge >= 18),
        HasValidLicense BIT NOT NULL,
        NumberOfDays INT NOT NULL CONSTRAINT CK_Bookings_Days CHECK (NumberOfDays BETWEEN 1 AND 28),
        CarType NVARCHAR(30) NOT NULL,
        FuelType NVARCHAR(30) NOT NULL,
        TotalCost DECIMAL(10,2) NOT NULL CONSTRAINT CK_Bookings_TotalCost CHECK (TotalCost >= 0),
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Bookings_CreatedAt DEFAULT (SYSDATETIME()),
        CONSTRAINT FK_Bookings_Cars FOREIGN KEY (CarId) REFERENCES dbo.Cars(CarId)
    );
END");

            // Create BookingOptionalExtras table if missing
            ExecuteNonQuery(connection, @"
IF OBJECT_ID(N'dbo.BookingOptionalExtras', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BookingOptionalExtras
    (
        BookingOptionalExtraId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_BookingOptionalExtras PRIMARY KEY,
        BookingId INT NOT NULL,
        ExtraName NVARCHAR(50) NOT NULL,
        DailyPrice DECIMAL(10,2) NOT NULL CONSTRAINT CK_BookingExtras_DailyPrice CHECK (DailyPrice >= 0),
        NumberOfDays INT NOT NULL CONSTRAINT CK_BookingExtras_Days CHECK (NumberOfDays BETWEEN 1 AND 28),
        CONSTRAINT FK_BookingExtras_Bookings FOREIGN KEY (BookingId) REFERENCES dbo.Bookings(BookingId) ON DELETE CASCADE
    );
END");
        }

        /// <summary>
        /// Non-destructively seeds the starter cars fleet without deleting or duplicating existing vehicles.
        /// Matches on unique RegistrationNumber to preserve user state.
        /// </summary>
        private static void SeedStarterFleet(SqlConnection connection)
        {
            var fleet = new[]
            {
                new { Reg = "WAC-001", Type = "CityCar",   Fuel = "Petrol" },
                new { Reg = "WAC-002", Type = "CityCar",   Fuel = "Diesel" },
                new { Reg = "WAC-003", Type = "FamilyCar", Fuel = "Petrol" },
                new { Reg = "WAC-004", Type = "FamilyCar", Fuel = "Diesel" },
                new { Reg = "WAC-005", Type = "SportsCar", Fuel = "Petrol" },
                new { Reg = "WAC-006", Type = "SportsCar", Fuel = "Hybrid" },
                new { Reg = "WAC-007", Type = "SUV",       Fuel = "Diesel" },
                new { Reg = "WAC-008", Type = "SUV",       Fuel = "Hybrid" },
                new { Reg = "WAC-009", Type = "SUV",       Fuel = "FullElectric" },
                new { Reg = "WAC-010", Type = "CityCar",   Fuel = "FullElectric" }
            };

            const string seedSql = @"
IF NOT EXISTS (SELECT 1 FROM dbo.Cars WHERE RegistrationNumber = @Reg)
BEGIN
    INSERT INTO dbo.Cars (RegistrationNumber, IsRented, DailyRate, CarType, FuelType)
    VALUES (@Reg, 0, 25.00, @CarType, @FuelType);
END";

            foreach (var car in fleet)
            {
                using (SqlCommand cmd = new SqlCommand(seedSql, connection))
                {
                    cmd.Parameters.Add("@Reg", SqlDbType.NVarChar, 20).Value = car.Reg;
                    cmd.Parameters.Add("@CarType", SqlDbType.NVarChar, 30).Value = car.Type;
                    cmd.Parameters.Add("@FuelType", SqlDbType.NVarChar, 30).Value = car.Fuel;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void ExecuteNonQuery(SqlConnection connection, string sql)
        {
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.CommandTimeout = 30;
                command.ExecuteNonQuery();
            }
        }
    }
}
