using System;
using System.Drawing;
using System.Windows.Forms;
using RentOnCars.Models;
using RentOnCars.Services;
using RentOnCars.UI;

namespace RentOnCars
{
    /// <summary>
    /// Modal dialog providing staff with an explicit review of customer details,
    /// allocated vehicle, itemized pricing breakdown, and extras before committing to the database.
    /// </summary>
    public partial class BookingReviewDialog : Form
    {
        private readonly Booking booking;
        private readonly PricingBreakdown pricing;

        public BookingReviewDialog(Booking booking, PricingBreakdown pricing)
        {
            this.booking = booking ?? throw new ArgumentNullException(nameof(booking));
            this.pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));

            InitializeComponent();
            PopulateReviewDetails();
        }

        private void PopulateReviewDetails()
        {
            // Customer Details
            customerNameLabel.Text = $"{booking.CustomerFirstName} {booking.CustomerSurname}";
            customerAddressLabel.Text = booking.CustomerAddress;
            customerAgeLabel.Text = $"{booking.CustomerAge} years old";
            customerLicenseLabel.Text = booking.HasValidLicense ? "Valid Driving Licence (Verified)" : "Invalid Licence";

            // Vehicle Details
            vehicleRegLabel.Text = booking.SelectedCar != null
                ? $"{booking.SelectedCar.RegistrationNumber} ({booking.SelectedCar.CarType} - {booking.SelectedCar.FuelType})"
                : "No vehicle assigned";
            rentalDaysLabel.Text = $"{booking.NumberOfDays} Day(s) @ £{pricing.DailyBaseRate:0.00} per day";

            // Itemized Bill
            baseRentItemLabel.Text = $"Base Fleet Hire ({booking.NumberOfDays} days × £{pricing.DailyBaseRate:0.00}):";
            baseRentValueLabel.Text = pricing.FormattedBaseRent;

            carSurchargeItemLabel.Text = $"Vehicle Surcharge ({booking.SelectedCar.CarType}):";
            carSurchargeValueLabel.Text = pricing.FormattedCarTypeSurcharge;

            fuelSurchargeItemLabel.Text = $"Fuel Surcharge ({booking.SelectedCar.FuelType}):";
            fuelSurchargeValueLabel.Text = pricing.FormattedFuelTypeSurcharge;

            string extrasDesc = "None";
            if (booking.Extras != null)
            {
                if (booking.Extras.UnlimitedMileage && booking.Extras.BreakdownCover)
                    extrasDesc = $"Unlimited Mileage (£{pricing.UnlimitedMileageCost:0.00}) + Breakdown Cover (£{pricing.BreakdownCoverCost:0.00})";
                else if (booking.Extras.UnlimitedMileage)
                    extrasDesc = $"Unlimited Mileage ({booking.NumberOfDays} days × £10.00 = £{pricing.UnlimitedMileageCost:0.00})";
                else if (booking.Extras.BreakdownCover)
                    extrasDesc = $"Breakdown Cover ({booking.NumberOfDays} days × £2.00 = £{pricing.BreakdownCoverCost:0.00})";
            }

            extrasItemLabel.Text = $"Optional Extras ({extrasDesc}):";
            extrasValueLabel.Text = pricing.FormattedExtrasTotal;

            totalCostValueLabel.Text = pricing.FormattedGrandTotal;
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
