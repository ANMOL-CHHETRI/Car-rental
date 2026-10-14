using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using RentOnCars.Models;

namespace RentOnCars.Repos
{
    public class Booking_repos
    {
        private readonly string connString =
            ConfigurationManager.ConnectionStrings["RentOnCarsDb"].ConnectionString;

        public int SaveBooking(Booking booking)
        {
            if (booking == null)
                throw new ArgumentNullException("booking");

            using (SqlConnection con = new SqlConnection(connString))
            {
                con.Open();

                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        // Make sure the selected car is still available.
                        string availabilitySql =
                            "SELECT COUNT(1) FROM Cars WHERE CarId = @CarId AND IsRented = 0;";

                        using (SqlCommand availabilityCmd = new SqlCommand(availabilitySql, con, transaction))
                        {
                            availabilityCmd.Parameters.Add("@CarId", SqlDbType.Int).Value = booking.CarId;
                            int available = Convert.ToInt32(availabilityCmd.ExecuteScalar());

                            if (available != 1)
                                throw new InvalidOperationException("The selected vehicle is no longer available.");
                        }

                        string bookingSql = @"
INSERT INTO Bookings
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
                            cmd.Parameters.Add("@CarId", SqlDbType.Int).Value = booking.CarId;
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

                        if (booking.Extras != null)
                        {
                            if (booking.Extras.UnlimitedMileage)
                                AddExtra(con, transaction, bookingId, "Unlimited Mileage", 10m, booking.NumberOfDays);

                            if (booking.Extras.BreakdownCover)
                                AddExtra(con, transaction, bookingId, "Breakdown Cover", 2m, booking.NumberOfDays);
                        }

                        string rentSql = @"
UPDATE Cars
SET IsRented = 1
WHERE CarId = @CarId AND IsRented = 0;";

                        using (SqlCommand rentCmd = new SqlCommand(rentSql, con, transaction))
                        {
                            rentCmd.Parameters.Add("@CarId", SqlDbType.Int).Value = booking.CarId;

                            if (rentCmd.ExecuteNonQuery() != 1)
                                throw new InvalidOperationException("The selected vehicle could not be marked as rented.");
                        }

                        transaction.Commit();
                        booking.BookingId = bookingId;
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
            string sql = @"
INSERT INTO BookingOptionalExtras (BookingId, ExtraName, DailyPrice, NumberOfDays)
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

        public List<Booking> GetDetails()
        {
            var bookings = new List<Booking>();

            using (SqlConnection con = new SqlConnection(connString))
            {
                con.Open();

                string sql = @"
SELECT
    b.BookingId, b.CarId, b.CustomerFirstName, b.CustomerSurname,
    b.CustomerAddress, b.CustomerAge, b.HasValidLicense,
    b.NumberOfDays, b.CarType, b.FuelType, b.TotalCost,
    c.RegistrationNumber, c.IsRented, c.DailyRate
FROM Bookings b
INNER JOIN Cars c ON c.CarId = b.CarId
ORDER BY b.BookingId DESC;";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        Booking booking = new Booking
                        {
                            BookingId = rd.GetInt32(0),
                            CarId = rd.GetInt32(1),
                            CustomerFirstName = rd.GetString(2),
                            CustomerSurname = rd.GetString(3),
                            CustomerAddress = rd.GetString(4),
                            CustomerAge = rd.GetInt32(5),
                            HasValidLicense = rd.GetBoolean(6),
                            NumberOfDays = rd.GetInt32(7),
                            TotalCost = rd.GetDecimal(10),
                            SelectedCar = new Car
                            {
                                CarId = rd.GetInt32(1),
                                RegistrationNumber = rd.GetString(11),
                                IsRented = rd.GetBoolean(12),
                                DailyRate = rd.GetDecimal(13),
                                CarType = ParseCarType(rd.GetString(8)),
                                FuelType = ParseFuelType(rd.GetString(9))
                            }
                        };

                        bookings.Add(booking);
                    }
                }
            }

            return bookings;
        }

        public List<Car> GetRentedCars()
        {
            var cars = new List<Car>();

            using (SqlConnection con = new SqlConnection(connString))
            {
                con.Open();

                string sql = @"
SELECT CarId, RegistrationNumber, IsRented, DailyRate, CarType, FuelType
FROM Cars
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
                            CarType = ParseCarType(rd.GetString(4)),
                            FuelType = ParseFuelType(rd.GetString(5))
                        });
                    }
                }
            }

            return cars;
        }

        public Car GetAvailableCar(CarType carType, FuelType fuelType)
        {
            using (SqlConnection con = new SqlConnection(connString))
            {
                con.Open();

                string sql = @"
SELECT TOP 1 CarId, RegistrationNumber, IsRented, DailyRate, CarType, FuelType
FROM Cars
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
                            CarType = ParseCarType(rd.GetString(4)),
                            FuelType = ParseFuelType(rd.GetString(5))
                        };
                    }
                }
            }
        }

        private CarType ParseCarType(string value)
        {
            CarType result;
            return Enum.TryParse(value, true, out result) ? result : CarType.CityCar;
        }

        private FuelType ParseFuelType(string value)
        {
            FuelType result;
            return Enum.TryParse(value, true, out result) ? result : FuelType.Petrol;
        }
    }
}
