using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using RentOnCars.Models;
using RentOnCars.Services;

namespace RentOnCars.Repos
{
    /// <summary>
    /// Dashboard summary metrics loaded from the database.
    /// </summary>
    public class DashboardStats
    {
        public int TotalVehicles { get; set; }
        public int AvailableVehicles { get; set; }
        public int RentedVehicles { get; set; }
        public int TotalBookings { get; set; }
    }

    /// <summary>
    /// Repository handling all database operations for WeAreCars using ADO.NET and parameterized SQL.
    /// Implements atomic transactional booking persistence, accurate extras retrieval, and concurrency safeguards.
    /// </summary>
    public class BookingRepository
    {
        private readonly string connectionString;

        public BookingRepository()
        {
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["RentOnCarsDb"];
            connectionString = setting != null && !string.IsNullOrWhiteSpace(setting.ConnectionString)
                ? setting.ConnectionString
                : "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=RentOnCarsDB;Integrated Security=True;Connect Timeout=30";
        }

        public BookingRepository(string customConnectionString)
        {
            connectionString = customConnectionString ?? throw new ArgumentNullException(nameof(customConnectionString));
        }

        /// <summary>
        /// Tests if the database connection can be opened successfully.
        /// </summary>
        public bool TestConnection(out string error)
        {
            error = null;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT 1;", con))
                    {
                        cmd.CommandTimeout = 5;
                        cmd.ExecuteScalar();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Retrieves live fleet counts and total bookings for the Dashboard.
        /// </summary>
        public DashboardStats GetDashboardStats()
        {
            DashboardStats stats = new DashboardStats();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                const string sql = @"
SELECT
    (SELECT COUNT(1) FROM dbo.Cars) AS TotalVehicles,
    (SELECT COUNT(1) FROM dbo.Cars WHERE IsRented = 0) AS AvailableVehicles,
    (SELECT COUNT(1) FROM dbo.Cars WHERE IsRented = 1) AS RentedVehicles,
    (SELECT COUNT(1) FROM dbo.Bookings) AS TotalBookings;";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        stats.TotalVehicles = rd.IsDBNull(0) ? 0 : rd.GetInt32(0);
                        stats.AvailableVehicles = rd.IsDBNull(1) ? 0 : rd.GetInt32(1);
                        stats.RentedVehicles = rd.IsDBNull(2) ? 0 : rd.GetInt32(2);
                        stats.TotalBookings = rd.IsDBNull(3) ? 0 : rd.GetInt32(3);
                    }
                }
            }

            return stats;
        }

        /// <summary>
        /// Finds the first available vehicle matching the requested category and fuel type.
        /// Returns null if no matching vehicles are currently unrented.
        /// </summary>
        public Car GetAvailableCar(CarType carType, FuelType fuelType)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                const string sql = @"
SELECT TOP 1 CarId, RegistrationNumber, IsRented, DailyRate, CarType, FuelType
FROM dbo.Cars
WHERE IsRented = 0 AND CarType = @CarType AND FuelType = @FuelType
ORDER BY CarId;";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@CarType", SqlDbType.NVarChar, 30).Value = carType.ToString();
                    cmd.Parameters.Add("@FuelType", SqlDbType.NVarChar, 30).Value = fuelType.ToString();

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (!rd.Read())
                            return null;

                        return new Car
                        {
                            CarId = rd.GetInt32(0),
                            RegistrationNumber = rd.GetString(1),
                            IsRented = rd.GetBoolean(2),
                            DailyRate = rd.GetDecimal(3),
                            CarType = VehicleEnumExtensions.ParseCarType(rd.GetString(4)),
                            FuelType = VehicleEnumExtensions.ParseFuelType(rd.GetString(5))
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Returns count of available vehicles matching category and fuel.
        /// </summary>
        public int GetAvailableCarsCount(CarType carType, FuelType fuelType)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                const string sql = @"
SELECT COUNT(1)
FROM dbo.Cars
WHERE IsRented = 0 AND CarType = @CarType AND FuelType = @FuelType;";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@CarType", SqlDbType.NVarChar, 30).Value = carType.ToString();
                    cmd.Parameters.Add("@FuelType", SqlDbType.NVarChar, 30).Value = fuelType.ToString();

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// Retrieves all currently rented vehicles (IsRented = 1).
        /// </summary>
        public List<Car> GetRentedCars()
        {
            var cars = new List<Car>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                const string sql = @"
SELECT CarId, RegistrationNumber, IsRented, DailyRate, CarType, FuelType
FROM dbo.Cars
WHERE IsRented = 1
ORDER BY RegistrationNumber;";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        cars.Add(new Car
                        {
                            CarId = rd.GetInt32(0),
                            RegistrationNumber = rd.GetString(1),
                            IsRented = rd.GetBoolean(2),
                            DailyRate = rd.GetDecimal(3),
                            CarType = VehicleEnumExtensions.ParseCarType(rd.GetString(4)),
                            FuelType = VehicleEnumExtensions.ParseFuelType(rd.GetString(5))
                        });
                    }
                }
            }

            return cars;
        }

        /// <summary>
        /// Retrieves all bookings including customer details, allocated vehicle information,
        /// and correctly populated saved optional extras.
        /// </summary>
        public List<Booking> GetDetails()
        {
            var bookings = new List<Booking>();
            var extrasMap = new Dictionary<int, OptionalExtras>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Step 1: Load all optional extras grouped by BookingId
                const string extrasSql = @"
SELECT BookingId, ExtraName
FROM dbo.BookingOptionalExtras
ORDER BY BookingId;";

                using (SqlCommand extrasCmd = new SqlCommand(extrasSql, con))
                using (SqlDataReader extrasRd = extrasCmd.ExecuteReader())
                {
                    while (extrasRd.Read())
                    {
                        int bookingId = extrasRd.GetInt32(0);
                        string extraName = extrasRd.GetString(1);

                        if (!extrasMap.TryGetValue(bookingId, out OptionalExtras extras))
                        {
                            extras = new OptionalExtras();
                            extrasMap[bookingId] = extras;
                        }

                        if (string.Equals(extraName, "Unlimited Mileage", StringComparison.OrdinalIgnoreCase))
                            extras.UnlimitedMileage = true;
                        else if (string.Equals(extraName, "Breakdown Cover", StringComparison.OrdinalIgnoreCase))
                            extras.BreakdownCover = true;
                    }
                }

                // Step 2: Load bookings with vehicle details
                const string sql = @"
SELECT
    b.BookingId, b.CarId, b.CustomerFirstName, b.CustomerSurname,
    b.CustomerAddress, b.CustomerAge, b.HasValidLicense,
    b.NumberOfDays, b.CarType, b.FuelType, b.TotalCost, b.CreatedAt,
    c.RegistrationNumber, c.IsRented, c.DailyRate
FROM dbo.Bookings b
INNER JOIN dbo.Cars c ON c.CarId = b.CarId
ORDER BY b.BookingId DESC;";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        int bookingId = rd.GetInt32(0);

                        OptionalExtras extras;
                        if (!extrasMap.TryGetValue(bookingId, out extras))
                        {
                            extras = new OptionalExtras();
                        }

                        Booking booking = new Booking
                        {
                            BookingId = bookingId,
                            CarId = rd.GetInt32(1),
                            CustomerFirstName = rd.GetString(2),
                            CustomerSurname = rd.GetString(3),
                            CustomerAddress = rd.GetString(4),
                            CustomerAge = rd.GetInt32(5),
                            HasValidLicense = rd.GetBoolean(6),
                            NumberOfDays = rd.GetInt32(7),
                            TotalCost = rd.GetDecimal(10),
                            CreatedAt = rd.GetDateTime(11),
                            Extras = extras,
                            SelectedCar = new Car
                            {
                                CarId = rd.GetInt32(1),
                                RegistrationNumber = rd.GetString(12),
                                IsRented = rd.GetBoolean(13),
                                DailyRate = rd.GetDecimal(14),
                                CarType = VehicleEnumExtensions.ParseCarType(rd.GetString(8)),
                                FuelType = VehicleEnumExtensions.ParseFuelType(rd.GetString(9))
                            }
                        };

                        bookings.Add(booking);
                    }
                }
            }

            return bookings;
        }

        /// <summary>
        /// Atomically saves a booking, its optional extras, and marks the selected car as rented.
        /// Protects against concurrent double-booking using row locks within the transaction,
        /// and recalculates total cost from validated rules to guarantee pricing integrity.
        /// </summary>
        public int SaveBooking(Booking booking)
        {
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));

            // Validate business rules and database string lengths at persistence boundary
            if (string.IsNullOrWhiteSpace(booking.CustomerFirstName))
                throw new ArgumentException("First name is required.", nameof(booking));
            if (booking.CustomerFirstName.Trim().Length > BookingValidator.MaxFirstNameLength)
                throw new ArgumentException($"First name exceeds {BookingValidator.MaxFirstNameLength} characters.", nameof(booking));

            if (string.IsNullOrWhiteSpace(booking.CustomerSurname))
                throw new ArgumentException("Surname is required.", nameof(booking));
            if (booking.CustomerSurname.Trim().Length > BookingValidator.MaxSurnameLength)
                throw new ArgumentException($"Surname exceeds {BookingValidator.MaxSurnameLength} characters.", nameof(booking));

            if (string.IsNullOrWhiteSpace(booking.CustomerAddress))
                throw new ArgumentException("Customer address is required.", nameof(booking));
            if (booking.CustomerAddress.Trim().Length > BookingValidator.MaxAddressLength)
                throw new ArgumentException($"Address exceeds {BookingValidator.MaxAddressLength} characters.", nameof(booking));

            if (booking.CustomerAge < BookingValidator.MinAge)
                throw new ArgumentException($"Customer must be at least {BookingValidator.MinAge} years old.", nameof(booking));

            if (!booking.HasValidLicense)
                throw new ArgumentException("A valid driving licence is required to hire a vehicle.", nameof(booking));

            if (booking.NumberOfDays < BookingValidator.MinRentalDays || booking.NumberOfDays > BookingValidator.MaxRentalDays)
                throw new ArgumentException($"Rental days must be between {BookingValidator.MinRentalDays} and {BookingValidator.MaxRentalDays}.", nameof(booking));

            if (booking.SelectedCar == null)
                throw new ArgumentException("A vehicle must be selected for the booking.", nameof(booking));

            // Derive authoritative total pricing from business rules
            decimal calculatedTotal = PricingCalculator.ComputeTotal(
                booking.NumberOfDays,
                booking.SelectedCar.CarType,
                booking.SelectedCar.FuelType,
                booking.Extras != null && booking.Extras.UnlimitedMileage,
                booking.Extras != null && booking.Extras.BreakdownCover,
                booking.SelectedCar.DailyRate > 0m ? booking.SelectedCar.DailyRate : PricingCalculator.BaseRentalDailyRate);

            booking.TotalCost = calculatedTotal;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                using (SqlTransaction transaction = con.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        // 1. Recheck availability and acquire update lock on the car
                        const string lockSql = @"
SELECT CarId, RegistrationNumber, DailyRate, CarType, FuelType
FROM dbo.Cars WITH (UPDLOCK, ROWLOCK, HOLDLOCK)
WHERE CarId = @CarId AND IsRented = 0;";

                        using (SqlCommand lockCmd = new SqlCommand(lockSql, con, transaction))
                        {
                            lockCmd.Parameters.Add("@CarId", SqlDbType.Int).Value = booking.SelectedCar.CarId;

                            using (SqlDataReader rd = lockCmd.ExecuteReader())
                            {
                                if (!rd.Read())
                                {
                                    throw new InvalidOperationException(
                                        $"The selected vehicle ({booking.SelectedCar.RegistrationNumber}) is no longer available. It may have just been rented by another staff member.");
                                }
                            }
                        }

                        // 2. Insert booking record
                        const string bookingSql = @"
INSERT INTO dbo.Bookings
(
    CarId, CustomerFirstName, CustomerSurname, CustomerAddress,
    CustomerAge, HasValidLicense, NumberOfDays, CarType, FuelType, TotalCost
)
VALUES
(
    @CarId, @CustomerFirstName, @CustomerSurname, @CustomerAddress,
    @CustomerAge, @HasValidLicense, @NumberOfDays, @CarType, @FuelType, @TotalCost
);
SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        int bookingId;
                        using (SqlCommand cmd = new SqlCommand(bookingSql, con, transaction))
                        {
                            cmd.Parameters.Add("@CarId", SqlDbType.Int).Value = booking.SelectedCar.CarId;
                            cmd.Parameters.Add("@CustomerFirstName", SqlDbType.NVarChar, 50).Value = booking.CustomerFirstName.Trim();
                            cmd.Parameters.Add("@CustomerSurname", SqlDbType.NVarChar, 50).Value = booking.CustomerSurname.Trim();
                            cmd.Parameters.Add("@CustomerAddress", SqlDbType.NVarChar, 150).Value = booking.CustomerAddress.Trim();
                            cmd.Parameters.Add("@CustomerAge", SqlDbType.Int).Value = booking.CustomerAge;
                            cmd.Parameters.Add("@HasValidLicense", SqlDbType.Bit).Value = booking.HasValidLicense;
                            cmd.Parameters.Add("@NumberOfDays", SqlDbType.Int).Value = booking.NumberOfDays;
                            cmd.Parameters.Add("@CarType", SqlDbType.NVarChar, 30).Value = booking.SelectedCar.CarType.ToString();
                            cmd.Parameters.Add("@FuelType", SqlDbType.NVarChar, 30).Value = booking.SelectedCar.FuelType.ToString();
                            cmd.Parameters.Add("@TotalCost", SqlDbType.Decimal).Value = booking.TotalCost;
                            cmd.Parameters["@TotalCost"].Precision = 10;
                            cmd.Parameters["@TotalCost"].Scale = 2;

                            bookingId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // 3. Insert optional extras if selected
                        if (booking.Extras != null)
                        {
                            if (booking.Extras.UnlimitedMileage)
                            {
                                AddExtra(con, transaction, bookingId, "Unlimited Mileage",
                                    OptionalExtras.UnlimitedMileageDailyRate, booking.NumberOfDays);
                            }

                            if (booking.Extras.BreakdownCover)
                            {
                                AddExtra(con, transaction, bookingId, "Breakdown Cover",
                                    OptionalExtras.BreakdownCoverDailyRate, booking.NumberOfDays);
                            }
                        }

                        // 4. Mark the vehicle as rented atomically
                        const string rentSql = @"
UPDATE dbo.Cars
SET IsRented = 1
WHERE CarId = @CarId AND IsRented = 0;";

                        using (SqlCommand rentCmd = new SqlCommand(rentSql, con, transaction))
                        {
                            rentCmd.Parameters.Add("@CarId", SqlDbType.Int).Value = booking.SelectedCar.CarId;
                            int rows = rentCmd.ExecuteNonQuery();

                            if (rows != 1)
                            {
                                throw new InvalidOperationException("Failed to update vehicle rental status. The transaction will be rolled back.");
                            }
                        }

                        // 5. Commit atomic transaction
                        transaction.Commit();

                        booking.BookingId = bookingId;
                        booking.SelectedCar.MarkAsRented();
                        return bookingId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private void AddExtra(
            SqlConnection con,
            SqlTransaction transaction,
            int bookingId,
            string extraName,
            decimal dailyPrice,
            int numberOfDays)
        {
            const string sql = @"
INSERT INTO dbo.BookingOptionalExtras (BookingId, ExtraName, DailyPrice, NumberOfDays)
VALUES (@BookingId, @ExtraName, @DailyPrice, @NumberOfDays);";

            using (SqlCommand cmd = new SqlCommand(sql, con, transaction))
            {
                cmd.Parameters.Add("@BookingId", SqlDbType.Int).Value = bookingId;
                cmd.Parameters.Add("@ExtraName", SqlDbType.NVarChar, 50).Value = extraName;
                cmd.Parameters.Add("@DailyPrice", SqlDbType.Decimal).Value = dailyPrice;
                cmd.Parameters["@DailyPrice"].Precision = 10;
                cmd.Parameters["@DailyPrice"].Scale = 2;
                cmd.Parameters.Add("@NumberOfDays", SqlDbType.Int).Value = numberOfDays;
                cmd.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Backward-compatibility class matching original repository naming (Booking_repos).
    /// Inherits directly from BookingRepository to guarantee existing references compile seamlessly.
    /// </summary>
    public class Booking_repos : BookingRepository
    {
        public Booking_repos() : base() { }
        public Booking_repos(string connString) : base(connString) { }
    }
}
