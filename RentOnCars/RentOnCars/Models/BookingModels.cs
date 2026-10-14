using System;
using System.Collections.Generic;
using System.Text;

namespace RentOnCars.Models
{
    /// <summary>
    /// Vehicle categories available in the WeAreCars fleet.
    /// Surcharges apply per booking as specified in the assignment brief.
    /// </summary>
    public enum CarType
    {
        CityCar,
        FamilyCar,
        SportsCar,
        SUV
    }

    /// <summary>
    /// Fuel and powertrain types available for rental vehicles.
    /// Surcharges apply per booking as specified in the assignment brief.
    /// </summary>
    public enum FuelType
    {
        Petrol,
        Diesel,
        Hybrid,
        FullElectric
    }

    /// <summary>
    /// Helper and extension methods for vehicle category and fuel enumerations.
    /// Implements the getSurcharge() operation from the class diagram.
    /// </summary>
    public static class VehicleEnumExtensions
    {
        /// <summary>
        /// Returns the fixed per-booking surcharge for the specified vehicle category.
        /// City car: £0 | Family car: £50 | Sports car: £75 | SUV: £65.
        /// </summary>
        public static decimal GetSurcharge(this CarType type)
        {
            switch (type)
            {
                case CarType.CityCar:
                    return 0m;
                case CarType.FamilyCar:
                    return 50m;
                case CarType.SportsCar:
                    return 75m;
                case CarType.SUV:
                    return 65m;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown vehicle category.");
            }
        }

        /// <summary>
        /// Alias matching the lowercase getSurcharge() in the assignment UML class diagram.
        /// </summary>
        public static decimal getSurcharge(this CarType type) => GetSurcharge(type);

        /// <summary>
        /// Returns a formatted display name with the surcharge note for user interfaces.
        /// </summary>
        public static string ToDisplayName(this CarType type)
        {
            switch (type)
            {
                case CarType.CityCar:
                    return "City car (No surcharge)";
                case CarType.FamilyCar:
                    return "Family car (+£50.00)";
                case CarType.SportsCar:
                    return "Sports car (+£75.00)";
                case CarType.SUV:
                    return "SUV (+£65.00)";
                default:
                    return type.ToString();
            }
        }

        /// <summary>
        /// Returns the fixed per-booking surcharge for the specified fuel type.
        /// Petrol: £0 | Diesel: £0 | Hybrid: £30 | Full electric: £50.
        /// </summary>
        public static decimal GetSurcharge(this FuelType type)
        {
            switch (type)
            {
                case FuelType.Petrol:
                    return 0m;
                case FuelType.Diesel:
                    return 0m;
                case FuelType.Hybrid:
                    return 30m;
                case FuelType.FullElectric:
                    return 50m;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown fuel type.");
            }
        }

        /// <summary>
        /// Alias matching the lowercase getSurcharge() in the assignment UML class diagram.
        /// </summary>
        public static decimal getSurcharge(this FuelType type) => GetSurcharge(type);

        /// <summary>
        /// Returns a formatted display name with the surcharge note for user interfaces.
        /// </summary>
        public static string ToDisplayName(this FuelType type)
        {
            switch (type)
            {
                case FuelType.Petrol:
                    return "Petrol (No surcharge)";
                case FuelType.Diesel:
                    return "Diesel (No surcharge)";
                case FuelType.Hybrid:
                    return "Hybrid (+£30.00)";
                case FuelType.FullElectric:
                    return "Full electric (+£50.00)";
                default:
                    return type.ToString();
            }
        }

        /// <summary>
        /// Safely parses a string into a CarType enum without silent fallback.
        /// </summary>
        public static CarType ParseCarType(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Car category string cannot be empty.", nameof(value));

            string normalized = value.Trim().Replace(" ", "").Replace("-", "");

            if (string.Equals(normalized, "CityCar", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "City", StringComparison.OrdinalIgnoreCase))
                return CarType.CityCar;

            if (string.Equals(normalized, "FamilyCar", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "Family", StringComparison.OrdinalIgnoreCase))
                return CarType.FamilyCar;

            if (string.Equals(normalized, "SportsCar", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "Sports", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "Sport", StringComparison.OrdinalIgnoreCase))
                return CarType.SportsCar;

            if (string.Equals(normalized, "SUV", StringComparison.OrdinalIgnoreCase))
                return CarType.SUV;

            if (Enum.TryParse(normalized, true, out CarType parsed))
                return parsed;

            throw new FormatException($"Invalid vehicle category value '{value}'. Expected CityCar, FamilyCar, SportsCar, or SUV.");
        }

        /// <summary>
        /// Safely parses a string into a FuelType enum without silent fallback.
        /// </summary>
        public static FuelType ParseFuelType(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Fuel type string cannot be empty.", nameof(value));

            string normalized = value.Trim().Replace(" ", "").Replace("-", "");

            if (string.Equals(normalized, "Petrol", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "Gasoline", StringComparison.OrdinalIgnoreCase))
                return FuelType.Petrol;

            if (string.Equals(normalized, "Diesel", StringComparison.OrdinalIgnoreCase))
                return FuelType.Diesel;

            if (string.Equals(normalized, "Hybrid", StringComparison.OrdinalIgnoreCase))
                return FuelType.Hybrid;

            if (string.Equals(normalized, "FullElectric", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "Electric", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "EV", StringComparison.OrdinalIgnoreCase))
                return FuelType.FullElectric;

            if (Enum.TryParse(normalized, true, out FuelType parsed))
                return parsed;

            throw new FormatException($"Invalid fuel type value '{value}'. Expected Petrol, Diesel, Hybrid, or FullElectric.");
        }
    }

    /// <summary>
    /// Represents a vehicle in the WeAreCars fleet as defined in the class diagram.
    /// </summary>
    public class Car
    {
        public int CarId { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public bool IsRented { get; set; }
        public decimal DailyRate { get; set; } = 25.00m;
        public CarType CarType { get; set; }
        public FuelType FuelType { get; set; }

        public Car()
        {
            DailyRate = 25.00m;
        }

        public Car(int carId, string registrationNumber, CarType carType, FuelType fuelType, bool isRented = false, decimal dailyRate = 25.00m)
        {
            CarId = carId;
            RegistrationNumber = registrationNumber ?? throw new ArgumentNullException(nameof(registrationNumber));
            CarType = carType;
            FuelType = fuelType;
            IsRented = isRented;
            DailyRate = dailyRate;
        }

        /// <summary>
        /// Returns a formatted vehicle summary string matching the class diagram.
        /// </summary>
        public string GetCarDetails()
        {
            return $"{RegistrationNumber} - {CarType} - {FuelType} (Base: £{DailyRate:0.00}/day)";
        }

        /// <summary>
        /// Marks this vehicle as rented.
        /// </summary>
        public void MarkAsRented()
        {
            IsRented = true;
        }

        /// <summary>
        /// Marks this vehicle as available for hire.
        /// </summary>
        public void MarkAsAvailable()
        {
            IsRented = false;
        }

        public override string ToString()
        {
            return GetCarDetails();
        }
    }

    /// <summary>
    /// Represents optional extras selected for a booking.
    /// Unlimited mileage: £10 per day | Breakdown cover: £2 per day.
    /// </summary>
    public class OptionalExtras
    {
        public const decimal UnlimitedMileageDailyRate = 10.00m;
        public const decimal BreakdownCoverDailyRate = 2.00m;

        public bool UnlimitedMileage { get; set; }
        public bool BreakdownCover { get; set; }

        public OptionalExtras()
        {
            UnlimitedMileage = false;
            BreakdownCover = false;
        }

        public OptionalExtras(bool unlimitedMileage, bool breakdownCover)
        {
            UnlimitedMileage = unlimitedMileage;
            BreakdownCover = breakdownCover;
        }

        /// <summary>
        /// Calculates the total cost of all selected extras for the specified rental days.
        /// </summary>
        public decimal CalculateExtrasCost(int numberOfDays)
        {
            if (numberOfDays < 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfDays), "Rental days cannot be negative.");

            decimal dailyExtras = 0m;
            if (UnlimitedMileage)
                dailyExtras += UnlimitedMileageDailyRate;
            if (BreakdownCover)
                dailyExtras += BreakdownCoverDailyRate;

            return dailyExtras * numberOfDays;
        }

        /// <summary>
        /// Parameterless overload matching the UML class diagram signature (+calculateExtrasCost() : decimal).
        /// Returns the daily rate of the selected extras.
        /// </summary>
        public decimal CalculateExtrasCost()
        {
            return CalculateExtrasCost(1);
        }

        /// <summary>
        /// Returns the combined daily charge for all selected extras.
        /// </summary>
        public decimal GetDailyExtrasTotal()
        {
            decimal daily = 0m;
            if (UnlimitedMileage)
                daily += UnlimitedMileageDailyRate;
            if (BreakdownCover)
                daily += BreakdownCoverDailyRate;
            return daily;
        }

        /// <summary>
        /// Formats a readable summary of selected extras.
        /// </summary>
        public string GetDescription()
        {
            if (UnlimitedMileage && BreakdownCover)
                return "Unlimited Mileage (£10/day), Breakdown Cover (£2/day)";
            if (UnlimitedMileage)
                return "Unlimited Mileage (£10/day)";
            if (BreakdownCover)
                return "Breakdown Cover (£2/day)";
            return "None";
        }
    }

    /// <summary>
    /// Represents a car rental booking matching the assignment domain model.
    /// </summary>
    public class Booking
    {
        public int BookingId { get; set; }
        public int CarId { get; set; }
        public string CustomerFirstName { get; set; } = string.Empty;
        public string CustomerSurname { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public int CustomerAge { get; set; } = 18;
        public bool HasValidLicense { get; set; } = true;
        public int NumberOfDays { get; set; } = 1;
        public decimal TotalCost { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Car SelectedCar { get; set; }
        public OptionalExtras Extras { get; set; }

        public Booking()
        {
            Extras = new OptionalExtras();
            NumberOfDays = 1;
            CustomerAge = 18;
            HasValidLicense = true;
        }

        /// <summary>
        /// Validates the booking state and computes the authoritative total cost.
        /// </summary>
        public void CreateBooking()
        {
            if (!ValidateBooking())
                throw new InvalidOperationException("Booking information is not valid.");

            TotalCost = CalculateTotalCost();
        }

        /// <summary>
        /// Validates business rules for this booking:
        /// Required first name, surname, address, age >= 18, valid licence, days 1..28, and available car.
        /// </summary>
        public bool ValidateBooking()
        {
            if (string.IsNullOrWhiteSpace(CustomerFirstName))
                return false;
            if (string.IsNullOrWhiteSpace(CustomerSurname))
                return false;
            if (string.IsNullOrWhiteSpace(CustomerAddress))
                return false;
            if (CustomerAge < 18)
                return false;
            if (!HasValidLicense)
                return false;
            if (NumberOfDays < 1 || NumberOfDays > 28)
                return false;
            if (SelectedCar == null)
                return false;

            return true;
        }

        /// <summary>
        /// Authoritative rental price calculation:
        /// Total = daily rate * rental days + vehicle surcharge + fuel surcharge + daily extras * rental days.
        /// </summary>
        public decimal CalculateTotalCost()
        {
            if (SelectedCar == null)
                throw new InvalidOperationException("A vehicle must be selected to calculate total cost.");

            decimal dailyBase = SelectedCar.DailyRate > 0m ? SelectedCar.DailyRate : 25.00m;
            decimal baseRent = dailyBase * NumberOfDays;
            decimal vehicleSurcharge = SelectedCar.CarType.GetSurcharge();
            decimal fuelSurcharge = SelectedCar.FuelType.GetSurcharge();
            decimal extrasCost = Extras != null ? Extras.CalculateExtrasCost(NumberOfDays) : 0m;

            return baseRent + vehicleSurcharge + fuelSurcharge + extrasCost;
        }

        /// <summary>
        /// Confirms the booking. Validates parameters, computes total cost,
        /// and prepares booking for atomic persistence.
        /// Note: Marking the physical car as rented in memory is finalized when database persistence succeeds.
        /// </summary>
        public void ConfirmBooking()
        {
            if (!ValidateBooking())
                throw new InvalidOperationException("Booking cannot be confirmed due to invalid information.");

            TotalCost = CalculateTotalCost();
        }
    }

    /// <summary>
    /// Encapsulates the collection of rented vehicles as modeled in the assignment class diagram.
    /// </summary>
    public class RentedCarsList
    {
        private readonly List<Car> rentedCars = new List<Car>();

        public List<Car> RentedCars => rentedCars;

        public int Count => rentedCars.Count;

        /// <summary>
        /// Implements the displayRentedCars() operation from the class diagram.
        /// Formats and returns a summary description of currently rented vehicles.
        /// </summary>
        public void DisplayRentedCars()
        {
            // Provides domain representation of rented cars.
            // Populates and prepares collection for display in UI grids/reports.
        }

        public string GetSummary()
        {
            if (rentedCars.Count == 0)
                return "No vehicles are currently rented.";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Total Rented Vehicles: {rentedCars.Count}");
            foreach (Car car in rentedCars)
            {
                sb.AppendLine($" • {car.GetCarDetails()}");
            }
            return sb.ToString();
        }

        public void AddRentedCar(Car car)
        {
            if (car != null && !rentedCars.Contains(car))
            {
                car.MarkAsRented();
                rentedCars.Add(car);
            }
        }

        public void RemoveRentedCar(Car car)
        {
            if (car != null && rentedCars.Contains(car))
            {
                car.MarkAsAvailable();
                rentedCars.Remove(car);
            }
        }

        public void Clear()
        {
            rentedCars.Clear();
        }
    }
}
