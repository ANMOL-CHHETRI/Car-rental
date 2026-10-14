using System;
using System.Drawing;
using System.Windows.Forms;
using RentOnCars.Models;
using RentOnCars.Repos;
using RentOnCars.Services;
using RentOnCars.UI;

namespace RentOnCars
{
    /// <summary>
    /// Registration and reservation screen for WeAreCars.
    /// Implements real-time live pricing independent of database access,
    /// dynamic fleet availability detection, inline validation, and explicit review before persistence.
    /// </summary>
    public partial class Details : Form
    {
        private readonly BookingRepository repository = new BookingRepository();
        private readonly ErrorProvider errorProvider = new ErrorProvider();

        private Car allocatedCar = null;
        private PricingBreakdown currentPricing = null;
        private bool isSaving = false;

        public Details()
        {
            InitializeComponent();
            ConfigureOptions();
            ApplyCustomStyling();
        }

        private void Details_Load(object sender, EventArgs e)
        {
            UpdateLivePricingAndAvailability();
        }

        private void ConfigureOptions()
        {
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            // Enforce domain constraints
            daysupdown.Minimum = 1;
            daysupdown.Maximum = 28;
            daysupdown.Value = 1;

            ageupdown.Minimum = 18;
            ageupdown.Maximum = 120;
            ageupdown.Value = 25;

            // Populate category combobox
            carcombobox.Items.Clear();
            carcombobox.Items.Add(CarType.CityCar.ToDisplayName());
            carcombobox.Items.Add(CarType.FamilyCar.ToDisplayName());
            carcombobox.Items.Add(CarType.SportsCar.ToDisplayName());
            carcombobox.Items.Add(CarType.SUV.ToDisplayName());

            // Populate fuel combobox
            fuelcombobox.Items.Clear();
            fuelcombobox.Items.Add(FuelType.Petrol.ToDisplayName());
            fuelcombobox.Items.Add(FuelType.Diesel.ToDisplayName());
            fuelcombobox.Items.Add(FuelType.Hybrid.ToDisplayName());
            fuelcombobox.Items.Add(FuelType.FullElectric.ToDisplayName());

            yesradiobutton.Checked = true;
        }

        private void ApplyCustomStyling()
        {
            Theme.ApplyPrimaryButton(proceedButton);
            Theme.ApplySecondaryButton(clearButton);
            Theme.ApplySecondaryButton(backButton);
        }

        #region Live Price Estimate & Availability

        private CarType? GetSelectedCarType()
        {
            if (carcombobox.SelectedIndex < 0)
                return null;

            switch (carcombobox.SelectedIndex)
            {
                case 0: return CarType.CityCar;
                case 1: return CarType.FamilyCar;
                case 2: return CarType.SportsCar;
                case 3: return CarType.SUV;
                default: return null;
            }
        }

        private FuelType? GetSelectedFuelType()
        {
            if (fuelcombobox.SelectedIndex < 0)
                return null;

            switch (fuelcombobox.SelectedIndex)
            {
                case 0: return FuelType.Petrol;
                case 1: return FuelType.Diesel;
                case 2: return FuelType.Hybrid;
                case 3: return FuelType.FullElectric;
                default: return null;
            }
        }

        /// <summary>
        /// Recalculates live itemized estimate and checks vehicle availability.
        /// Price calculation is strictly independent of database access.
        /// </summary>
        public void UpdateLivePricingAndAvailability()
        {
            int days = (int)daysupdown.Value;
            CarType? carType = GetSelectedCarType();
            FuelType? fuelType = GetSelectedFuelType();
            bool unlimitedMileage = umcheckbox.Checked;
            bool breakdownCover = bccheckbox.Checked;

            // 1. Calculate live estimate independently of the database
            currentPricing = PricingCalculator.CalculateEstimate(
                days, carType, fuelType, unlimitedMileage, breakdownCover);

            // Update live estimate UI panel
            estimateDaysLabel.Text = $"{currentPricing.RentalDays} Day(s) @ £{currentPricing.DailyBaseRate:0.00}/day";
            estimateBaseValueLabel.Text = currentPricing.FormattedBaseRent;

            estimateCarTypeValueLabel.Text = carType.HasValue
                ? currentPricing.FormattedCarTypeSurcharge
                : "£0.00 (Select Category)";

            estimateFuelTypeValueLabel.Text = fuelType.HasValue
                ? currentPricing.FormattedFuelTypeSurcharge
                : "£0.00 (Select Fuel)";

            estimateExtrasValueLabel.Text = currentPricing.FormattedExtrasTotal;
            estimateTotalValueLabel.Text = currentPricing.FormattedGrandTotal;

            if (currentPricing.IsComplete)
            {
                estimateStatusLabel.Text = "● Ready for Review";
                estimateStatusLabel.ForeColor = Theme.SuccessGreen;
            }
            else
            {
                estimateStatusLabel.Text = "▲ " + currentPricing.StatusMessage;
                estimateStatusLabel.ForeColor = Theme.WarningAmber;
            }

            // 2. Query vehicle availability if both category and fuel are selected
            CheckVehicleAvailability(carType, fuelType);

            // 3. Update confirm button state
            ValidateFormInline(silent: true);
        }

        private void CheckVehicleAvailability(CarType? carType, FuelType? fuelType)
        {
            allocatedCar = null;

            if (!carType.HasValue || !fuelType.HasValue)
            {
                availabilityStatusLabel.Text = "Select vehicle category and fuel type to check fleet availability.";
                availabilityStatusLabel.ForeColor = Theme.TextMuted;
                availabilityBadgePanel.BackColor = Color.FromArgb(241, 245, 249);
                return;
            }

            try
            {
                allocatedCar = repository.GetAvailableCar(carType.Value, fuelType.Value);

                if (allocatedCar != null)
                {
                    availabilityStatusLabel.Text = $"Available: Allocated Registration [{allocatedCar.RegistrationNumber}]";
                    availabilityStatusLabel.ForeColor = Theme.SuccessGreen;
                    availabilityBadgePanel.BackColor = Theme.SuccessGreenBg;
                }
                else
                {
                    availabilityStatusLabel.Text = $"Unavailable: All {carType.Value} ({fuelType.Value}) vehicles are currently rented.";
                    availabilityStatusLabel.ForeColor = Theme.DangerRed;
                    availabilityBadgePanel.BackColor = Theme.DangerRedBg;
                }
            }
            catch (Exception)
            {
                availabilityStatusLabel.Text = "Database offline: Fleet availability cannot be verified.";
                availabilityStatusLabel.ForeColor = Theme.WarningAmber;
                availabilityBadgePanel.BackColor = Theme.WarningAmberBg;
            }
        }

        #endregion

        #region Form Validation

        /// <summary>
        /// Validates form inputs inline using ErrorProvider, avoiding disruptive popups while typing.
        /// </summary>
        private bool ValidateFormInline(bool silent = false)
        {
            errorProvider.Clear();

            string firstName = firstnamebox.Text.Trim();
            string surname = lastnamebox.Text.Trim();
            string address = addressbox.Text.Trim();
            int age = (int)ageupdown.Value;
            bool hasLicense = yesradiobutton.Checked;
            int days = (int)daysupdown.Value;
            CarType? carType = GetSelectedCarType();
            FuelType? fuelType = GetSelectedFuelType();

            BookingValidationResult result = BookingValidator.Validate(
                firstName, surname, address, age, hasLicense, days, carType, fuelType, allocatedCar);

            // Apply inline error icons
            if (result.FieldErrors.ContainsKey("FirstName"))
                errorProvider.SetError(firstnamebox, result.GetError("FirstName"));

            if (result.FieldErrors.ContainsKey("Surname"))
                errorProvider.SetError(lastnamebox, result.GetError("Surname"));

            if (result.FieldErrors.ContainsKey("Address"))
                errorProvider.SetError(addressbox, result.GetError("Address"));

            if (result.FieldErrors.ContainsKey("Age"))
                errorProvider.SetError(ageupdown, result.GetError("Age"));

            if (result.FieldErrors.ContainsKey("License"))
                errorProvider.SetError(licensePanel, result.GetError("License"));

            if (result.FieldErrors.ContainsKey("RentalDays"))
                errorProvider.SetError(daysupdown, result.GetError("RentalDays"));

            if (result.FieldErrors.ContainsKey("CarType"))
                errorProvider.SetError(carcombobox, result.GetError("CarType"));

            if (result.FieldErrors.ContainsKey("FuelType"))
                errorProvider.SetError(fuelcombobox, result.GetError("FuelType"));

            bool canProceed = result.IsValid && allocatedCar != null && !isSaving;
            proceedButton.Enabled = canProceed;

            if (!silent && !result.IsValid)
            {
                MessageBox.Show(
                    result.BuildSummary(),
                    "WeAreCars - Incomplete Booking Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            return result.IsValid && allocatedCar != null;
        }

        #endregion

        #region Booking Submission & Review Workflow

        private void proceedButton_Click(object sender, EventArgs e)
        {
            if (isSaving)
                return;

            if (!ValidateFormInline(silent: false))
                return;

            if (allocatedCar == null)
            {
                MessageBox.Show(
                    "No vehicle is currently available for the selected category and fuel type.\n" +
                    "Please choose another category or fuel type.",
                    "Vehicle Unavailable",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Build booking domain object
            Booking booking = new Booking
            {
                CustomerFirstName = firstnamebox.Text.Trim(),
                CustomerSurname = lastnamebox.Text.Trim(),
                CustomerAddress = addressbox.Text.Trim(),
                CustomerAge = (int)ageupdown.Value,
                HasValidLicense = yesradiobutton.Checked,
                NumberOfDays = (int)daysupdown.Value,
                SelectedCar = allocatedCar,
                CarId = allocatedCar.CarId,
                Extras = new OptionalExtras
                {
                    UnlimitedMileage = umcheckbox.Checked,
                    BreakdownCover = bccheckbox.Checked
                }
            };

            // Calculate authoritative total
            booking.CreateBooking();

            // Step: Let staff review details, vehicle, days, extras, and total before saving
            using (BookingReviewDialog reviewDialog = new BookingReviewDialog(booking, currentPricing))
            {
                DialogResult dialogResult = reviewDialog.ShowDialog(this);

                // If staff chose "Return to Editing", do not save and retain all entered inputs
                if (dialogResult != DialogResult.OK)
                    return;
            }

            // Explicit confirmation received: Save atomically
            SaveBookingToDatabase(booking);
        }

        private void SaveBookingToDatabase(Booking booking)
        {
            try
            {
                isSaving = true;
                proceedButton.Enabled = false;
                Cursor = Cursors.WaitCursor;

                // Rechecks availability during transaction, saves booking + extras, and marks car rented atomically
                int bookingId = repository.SaveBooking(booking);

                // Display official receipt with saved booking ID after success
                DisplayBookingReceipt(booking, bookingId);

                // Clear form only upon successful save
                ClearForm();
            }
            catch (Exception ex)
            {
                // Retain entered information if saving fails
                MessageBox.Show(
                    "Unable to complete the booking.\n\n" +
                    ex.Message + "\n\n" +
                    "Your entered customer information has been retained. Please try again or select another available vehicle.",
                    "Booking Persistence Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                isSaving = false;
                Cursor = Cursors.Default;
                UpdateLivePricingAndAvailability();
            }
        }

        private void DisplayBookingReceipt(Booking booking, int bookingId)
        {
            string extras = booking.Extras != null ? booking.Extras.GetDescription() : "None";

            string receipt =
                "====================================================\n" +
                "               WEARECARS RENTAL RECEIPT             \n" +
                "====================================================\n\n" +
                $"Booking Reference : WAC-BK-{bookingId:D5}\n" +
                $"Date & Time       : {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                $"Staff Member      : sta001\n\n" +
                "--- CUSTOMER DETAILS ---\n" +
                $"Name              : {booking.CustomerFirstName} {booking.CustomerSurname}\n" +
                $"Address           : {booking.CustomerAddress}\n" +
                $"Age               : {booking.CustomerAge} (Licence: Valid)\n\n" +
                "--- VEHICLE & HIRE DETAILS ---\n" +
                $"Vehicle Reg       : {booking.SelectedCar.RegistrationNumber}\n" +
                $"Category          : {booking.SelectedCar.CarType}\n" +
                $"Fuel Type         : {booking.SelectedCar.FuelType}\n" +
                $"Rental Duration   : {booking.NumberOfDays} Day(s)\n" +
                $"Optional Extras   : {extras}\n\n" +
                "--- FINANCIAL BREAKDOWN ---\n" +
                $"Base Fleet Hire   : £{currentPricing.BaseRentTotal:0.00}\n" +
                $"Vehicle Surcharge : £{currentPricing.CarTypeSurcharge:0.00}\n" +
                $"Fuel Surcharge    : £{currentPricing.FuelTypeSurcharge:0.00}\n" +
                $"Extras Surcharge  : £{currentPricing.ExtrasTotal:0.00}\n" +
                "----------------------------------------------------\n" +
                $"TOTAL AMOUNT PAID : £{booking.TotalCost:0.00} GBP\n" +
                "====================================================\n\n" +
                "Vehicle status updated to HIRED. Have a safe journey!";

            MessageBox.Show(
                receipt,
                "WeAreCars - Booking Confirmed & Receipt Issued",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void ClearForm()
        {
            firstnamebox.Clear();
            lastnamebox.Clear();
            addressbox.Clear();
            ageupdown.Value = 25;
            daysupdown.Value = 1;
            yesradiobutton.Checked = true;
            noradiobutton.Checked = false;
            carcombobox.SelectedIndex = -1;
            fuelcombobox.SelectedIndex = -1;
            umcheckbox.Checked = false;
            bccheckbox.Checked = false;
            errorProvider.Clear();
            allocatedCar = null;

            firstnamebox.Focus();
        }

        #endregion

        #region Event Handlers for Real-Time Updates

        private void OptionChanged(object sender, EventArgs e)
        {
            UpdateLivePricingAndAvailability();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to clear all entered customer details and rental options?",
                "Clear Form",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                ClearForm();
                UpdateLivePricingAndAvailability();
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion
    }
}
