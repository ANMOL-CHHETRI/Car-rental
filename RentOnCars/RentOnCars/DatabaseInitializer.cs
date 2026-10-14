using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

namespace RentOnCars
{
    /// <summary>
    /// Automatically prepares the local SQL Server LocalDB database.
    /// No manual SQL script execution is required by the user.
    /// </summary>
    internal static class DatabaseInitializer
    {
        private const string DatabaseName = "RentOnCarsDB";
        private const string LocalDbInstance = "MSSQLLocalDB";

        private static string ConnectionString
        {
            get
            {
                ConnectionStringSettings setting =
                    ConfigurationManager.ConnectionStrings["RentOnCarsDb"];

                if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
                    throw new InvalidOperationException("The RentOnCarsDb connection string is missing from App.config.");

                return setting.ConnectionString;
            }
        }

        public static void Initialize()
        {
            // Visual Studio normally installs SqlLocalDB.exe. If the instance has
            // not been created/started yet, start it automatically.
            EnsureLocalDbInstance();

            string masterConnectionString = new SqlConnectionStringBuilder(ConnectionString)
            {
                InitialCatalog = "master",
                ConnectTimeout = 15
            }.ConnectionString;

            using (SqlConnection master = new SqlConnection(masterConnectionString))
            {
                master.Open();

                using (SqlCommand command = new SqlCommand(
                    "IF DB_ID(@DatabaseName) IS NULL CREATE DATABASE [RentOnCarsDB];", master))
                {
                    command.CommandTimeout = 30;
                    command.Parameters.Add("@DatabaseName", SqlDbType.NVarChar, 128).Value = DatabaseName;
                    command.ExecuteNonQuery();
                }
            }

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                CreateTables(connection);
                SeedCars(connection);
            }
        }

        private static void EnsureLocalDbInstance()
        {
            string sqlLocalDb = FindSqlLocalDbExecutable();

            // If SqlLocalDB.exe is not installed, the normal connection attempt
            // below will produce the useful SQL Server error instead of hiding it.
            if (string.IsNullOrEmpty(sqlLocalDb))
                return;

            string info = RunSqlLocalDb(sqlLocalDb, "info " + LocalDbInstance);

            if (string.IsNullOrWhiteSpace(info) ||
                info.IndexOf("LocalDB instance is not running", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                // If the instance does not exist, create it. If it already exists,
                // create simply returns an error which we safely ignore and then start it.
                RunSqlLocalDb(sqlLocalDb, "create " + LocalDbInstance);
            }

            RunSqlLocalDb(sqlLocalDb, "start " + LocalDbInstance);
        }

        private static string FindSqlLocalDbExecutable()
        {
            string[] candidates =
            {
                "SqlLocalDB.exe",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft SQL Server", "170", "Tools", "Binn", "SqlLocalDB.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft SQL Server", "160", "Tools", "Binn", "SqlLocalDB.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft SQL Server", "150", "Tools", "Binn", "SqlLocalDB.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft SQL Server", "140", "Tools", "Binn", "SqlLocalDB.exe")
            };

            foreach (string candidate in candidates)
            {
                if (candidate == "SqlLocalDB.exe")
                    return candidate;

                if (File.Exists(candidate))
                    return candidate;
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
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit(10000);
                    return output + Environment.NewLine + error;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private static void CreateTables(SqlConnection connection)
        {
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

        private static void SeedCars(SqlConnection connection)
        {
            ExecuteNonQuery(connection, @"
IF NOT EXISTS (SELECT 1 FROM dbo.Cars)
BEGIN
    INSERT INTO dbo.Cars (RegistrationNumber, IsRented, DailyRate, CarType, FuelType)
    VALUES
    (N'WAC-001', 0, 25.00, N'CityCar',   N'Petrol'),
    (N'WAC-002', 0, 25.00, N'CityCar',   N'Diesel'),
    (N'WAC-003', 0, 25.00, N'FamilyCar', N'Petrol'),
    (N'WAC-004', 0, 25.00, N'FamilyCar', N'Diesel'),
    (N'WAC-005', 0, 25.00, N'SportsCar', N'Petrol'),
    (N'WAC-006', 0, 25.00, N'SportsCar', N'Hybrid'),
    (N'WAC-007', 0, 25.00, N'SUV',       N'Diesel'),
    (N'WAC-008', 0, 25.00, N'SUV',       N'Hybrid'),
    (N'WAC-009', 0, 25.00, N'SUV',       N'FullElectric'),
    (N'WAC-010', 0, 25.00, N'CityCar',   N'FullElectric');
END");
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
