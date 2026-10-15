using System;
using System.Globalization;
using RentOnCars.Models;

namespace RentOnCars.Services
{
    /// <summary>
    /// Itemized breakdown of a car rental price calculation.
    /// Provides transparent financial details for UI display, estimates, and receipts.
    /// </summary>
    public class PricingBreakdown
    {
        public int RentalDays { get; set; }
        public decimal DailyBaseRate { get; set; } = 25.00m;
        public decimal BaseRentTotal => DailyBaseRate * RentalDays;

        public CarType? SelectedCarType { get; set; }
        public decimal CarTypeSurcharge { get; set; }

        public FuelType? SelectedFuelType { get; set; }
        public decimal FuelTypeSurcharge { get; set; }

        public bool UnlimitedMileageSelected { get; set; }
        public decimal UnlimitedMileageRate { get; set; } = 10.00m;
        public decimal UnlimitedMileageCost => UnlimitedMileageSelected ? UnlimitedMileageRate * RentalDays : 0m;

        public bool BreakdownCoverSelected { get; set; }
        public decimal BreakdownCoverRate { get; set; } = 2.00m;
        public decimal BreakdownCoverCost => BreakdownCoverSelected ? BreakdownCoverRate * RentalDays : 0m;

        public decimal DailyExtrasTotal => (UnlimitedMileageSelected ? UnlimitedMileageRate : 0m) +
                                           (BreakdownCoverSelected ? BreakdownCoverRate : 0m);

        public decimal ExtrasTotal => UnlimitedMileageCost + BreakdownCoverCost;

        public decimal GrandTotal => BaseRentTotal + CarTypeSurcharge + FuelTypeSurcharge + ExtrasTotal;

        public bool IsComplete { get; set; }
        public string StatusMessage { get; set; } = string.Empty;

        public string FormattedGrandTotal => PricingCalculator.FormatMoney(GrandTotal);
        public string FormattedBaseRent => PricingCalculator.FormatMoney(BaseRentTotal);
        public string FormattedCarTypeSurcharge => CarTypeSurcharge > 0m ? "+" + PricingCalculator.FormatMoney(CarTypeSurcharge) : "£0.00 (Included)";
        public string FormattedFuelTypeSurcharge => FuelTypeSurcharge > 0m ? "+" + PricingCalculator.FormatMoney(FuelTypeSurcharge) : "£0.00 (Included)";
        public string FormattedExtrasTotal => PricingCalculator.FormatMoney(ExtrasTotal);
    }

    /// <summary>
    /// Dedicated pricing service for calculating rental fees and estimates independently of database access.
    /// Follows the assignment charges strictly:
    /// - Base rental: £25 per day
    /// - City car: £0 | Family car: £50 | Sports car: £75 | SUV: £65
    /// - Petrol: £0 | Diesel: £0 | Hybrid: £30 | Full electric: £50
    /// - Unlimited mileage: £10 per day | Breakdown cover: £2 per day
    /// </summary>
    public static class PricingCalculator
    {
        public static string FormatMoney(decimal amount) => amount.ToString("C2", CultureInfo.GetCultureInfo("en-GB"));

        public const decimal BaseRentalDailyRate = 25.00m;
        public const decimal UnlimitedMileageDailyRate = 10.00m;
        public const decimal BreakdownCoverDailyRate = 2.00m;

        /// <summary>
        /// Calculates a live estimate breakdown based on the currently selected options.
        /// Works entirely offline without requiring database access.
        /// </summary>
        /// <param name="rentalDays">Rental duration in days (1 to 28).</param>
        /// <param name="carType">Selected vehicle category, or null if unselected.</param>
        /// <param name="fuelType">Selected fuel type, or null if unselected.</param>
        /// <param name="unlimitedMileage">Whether unlimited mileage extra is selected.</param>
        /// <param name="breakdownCover">Whether breakdown cover extra is selected.</param>
        /// <returns>A populated PricingBreakdown with itemized fees and completion status.</returns>
        public static PricingBreakdown CalculateEstimate(
            int rentalDays,
            CarType? carType,
            FuelType? fuelType,
            bool unlimitedMileage,
            bool breakdownCover,
            decimal dailyBaseRate = BaseRentalDailyRate)
        {
            if (dailyBaseRate <= 0m) throw new ArgumentOutOfRangeException(nameof(dailyBaseRate));
            int safeDays = Math.Max(1, Math.Min(28, rentalDays));

            PricingBreakdown breakdown = new PricingBreakdown
            {
                RentalDays = safeDays,
                DailyBaseRate = dailyBaseRate,
                SelectedCarType = carType,
                SelectedFuelType = fuelType,
                UnlimitedMileageSelected = unlimitedMileage,
                BreakdownCoverSelected = breakdownCover,
                UnlimitedMileageRate = UnlimitedMileageDailyRate,
                BreakdownCoverRate = BreakdownCoverDailyRate
            };

            bool hasCategory = carType.HasValue;
            bool hasFuel = fuelType.HasValue;

            if (hasCategory)
            {
                breakdown.CarTypeSurcharge = carType.Value.GetSurcharge();
            }

            if (hasFuel)
            {
                breakdown.FuelTypeSurcharge = fuelType.Value.GetSurcharge();
            }

            if (hasCategory && hasFuel && rentalDays >= 1 && rentalDays <= 28)
            {
                breakdown.IsComplete = true;
                breakdown.StatusMessage = "Estimate complete";
            }
            else if (rentalDays < 1 || rentalDays > 28)
            {
                breakdown.IsComplete = false;
                breakdown.StatusMessage = "Rental duration must be between 1 and 28 whole days";
            }
            else if (!hasCategory && !hasFuel)
            {
                breakdown.IsComplete = false;
                breakdown.StatusMessage = "Incomplete: Select car category and fuel type";
            }
            else if (!hasCategory)
            {
                breakdown.IsComplete = false;
                breakdown.StatusMessage = "Incomplete: Select car category";
            }
            else
            {
                breakdown.IsComplete = false;
                breakdown.StatusMessage = "Incomplete: Select fuel type";
            }

            return breakdown;
        }

        /// <summary>
        /// Computes the exact total cost using decimal arithmetic.
        /// Formula: dailyRate * rentalDays + vehicleSurcharge + fuelSurcharge + dailyExtras * rentalDays.
        /// Example: 3 days SUV Hybrid with both extras:
        /// 25 * 3 (75) + 65 (SUV) + 30 (Hybrid) + (10 + 2) * 3 (36) = £206.00.
        /// </summary>
        public static decimal ComputeTotal(
            int rentalDays,
            CarType carType,
            FuelType fuelType,
            bool unlimitedMileage,
            bool breakdownCover,
            decimal dailyBaseRate = BaseRentalDailyRate)
        {
            if (rentalDays < 1 || rentalDays > 28)
                throw new ArgumentOutOfRangeException(nameof(rentalDays), "Rental days must be between 1 and 28.");

            if (dailyBaseRate <= 0m) throw new ArgumentOutOfRangeException(nameof(dailyBaseRate), "Daily rate must be positive.");
            decimal baseRent = dailyBaseRate * rentalDays;
            decimal carSurcharge = carType.GetSurcharge();
            decimal fuelSurcharge = fuelType.GetSurcharge();

            decimal dailyExtras = 0m;
            if (unlimitedMileage)
                dailyExtras += UnlimitedMileageDailyRate;
            if (breakdownCover)
                dailyExtras += BreakdownCoverDailyRate;

            decimal extrasTotal = dailyExtras * rentalDays;

            return baseRent + carSurcharge + fuelSurcharge + extrasTotal;
        }
    }
}
