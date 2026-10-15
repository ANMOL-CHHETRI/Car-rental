using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RentOnCars.Models;

namespace RentOnCars.Services
{
    /// <summary>
    /// Represents the outcome of booking input validation.
    /// Provides field-specific error messages for inline UI feedback as well as overall validity.
    /// </summary>
    public class BookingValidationResult
    {
        private readonly Dictionary<string, string> fieldErrors = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public bool IsValid => fieldErrors.Count == 0;

        public IReadOnlyDictionary<string, string> FieldErrors => fieldErrors;

        public void AddError(string fieldName, string errorMessage)
        {
            if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(errorMessage))
            {
                fieldErrors[fieldName] = errorMessage;
            }
        }

        public string GetError(string fieldName)
        {
            if (fieldErrors.TryGetValue(fieldName, out string error))
                return error;
            return null;
        }

        public string BuildSummary()
        {
            if (IsValid)
                return "All inputs are valid.";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Please resolve the following validation issues:");
            foreach (var kvp in fieldErrors)
            {
                sb.AppendLine($" • {kvp.Value}");
            }
            return sb.ToString().TrimEnd();
        }
    }

    /// <summary>
    /// Service for validating customer and rental inputs against domain rules and database schema limits.
    /// </summary>
    public static class BookingValidator
    {
        public const int MaxFirstNameLength = 50;
        public const int MaxSurnameLength = 50;
        public const int MaxAddressLength = 150;
        public const int MinAge = 18;
        public const int MinRentalDays = 1;
        public const int MaxRentalDays = 28;

        /// <summary>
        /// Validates all booking fields and returns detailed field-level feedback.
        /// </summary>
        public static BookingValidationResult Validate(
            string firstName,
            string surname,
            string address,
            int age,
            bool hasValidLicense,
            int rentalDays,
            CarType? carType,
            FuelType? fuelType,
            Car availableCar = null)
        {
            BookingValidationResult result = new BookingValidationResult();

            // First name validation
            if (string.IsNullOrWhiteSpace(firstName))
            {
                result.AddError("FirstName", "First name is required.");
            }
            else if (firstName.Trim().Length > MaxFirstNameLength)
            {
                result.AddError("FirstName", $"First name cannot exceed {MaxFirstNameLength} characters (currently {firstName.Trim().Length}).");
            }

            // Surname validation (mandatory surname rule)
            if (string.IsNullOrWhiteSpace(surname))
            {
                result.AddError("Surname", "Surname is required.");
            }
            else if (surname.Trim().Length > MaxSurnameLength)
            {
                result.AddError("Surname", $"Surname cannot exceed {MaxSurnameLength} characters (currently {surname.Trim().Length}).");
            }

            // Address validation
            if (string.IsNullOrWhiteSpace(address))
            {
                result.AddError("Address", "Customer address is required.");
            }
            else if (address.Trim().Length > MaxAddressLength)
            {
                result.AddError("Address", $"Address cannot exceed {MaxAddressLength} characters (currently {address.Trim().Length}).");
            }

            // Age validation
            if (age < MinAge || age > 120)
            {
                result.AddError("Age", $"Customer must be at least {MinAge} years old to rent a vehicle (entered: {age}).");
            }

            // Licence confirmation
            if (!hasValidLicense)
            {
                result.AddError("License", "A valid driving licence is mandatory. Customer cannot rent without a licence.");
            }

            // Rental days
            if (rentalDays < MinRentalDays || rentalDays > MaxRentalDays)
            {
                result.AddError("RentalDays", $"Rental duration must be between {MinRentalDays} and {MaxRentalDays} days (entered: {rentalDays}).");
            }

            // Category selection
            if (!carType.HasValue || !Enum.IsDefined(typeof(CarType), carType.Value))
            {
                result.AddError("CarType", "Please select a vehicle category.");
            }

            // Fuel type selection
            if (!fuelType.HasValue || !Enum.IsDefined(typeof(FuelType), fuelType.Value))
            {
                result.AddError("FuelType", "Please select a fuel type.");
            }

            // Vehicle availability
            if (carType.HasValue && fuelType.HasValue && (availableCar == null || availableCar.IsRented || availableCar.CarType != carType.Value || availableCar.FuelType != fuelType.Value || availableCar.DailyRate <= 0m))
            {
                result.AddError("VehicleAvailability", "No available vehicle matches the selected category and fuel type.");
            }

            return result;
        }
    }
}
