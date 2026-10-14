using System;
using System.Windows.Forms;
using RentOnCars.Models;
using RentOnCars.Repos;

namespace RentOnCars
{
    public partial class Details : Form
    {
        private readonly Booking_repos repository = new Booking_repos();

        public Details()
        {
            InitializeComponent();
            ConfigureOptions();
        }

        private void ConfigureOptions()
        {
            // The report specifies 1 to 28 rental days.
            daysupdown.Minimum = 1;
            daysupdown.Maximum = 28;

            // Legal rental age requirement.
            ageupdown.Minimum = 18;

            if (carcombobox.Items.Count == 0)
            {
                carcombobox.Items.AddRange(new object[]
                {
                    "City car",
                    "Family car",
                    "Sports car",
                    "SUV"
                });
            }

            if (fuelcombobox.Items.Count == 0)
            {
                fuelcombobox.Items.AddRange(new object[]
                {
                    "Petrol",
                    "Diesel",
                    "Hybrid",
                    "Full electric"
                });
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Booking booking = BuildBooking();

                if (!booking.ValidateBooking())
                    return;

                booking.CreateBooking();

                // The selected car is part of the Booking class and is persisted
                // together with the booking in one database transaction.
                repository.SaveBooking(booking);

                ShowBill(booking);

                MessageBox.Show(
                    "Booking saved successfully and the selected car is now marked as rented.",
                    "Booking Confirmed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to save the booking.\n\n" + ex.Message,
                    "Booking Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private Booking BuildBooking()
        {
            string firstname = firstnamebox.Text.Trim();
            string surname = lastnamebox.Text.Trim();
            string address = addressbox.Text.Trim();
            bool hasLicense = yesradiobutton.Checked;

            if (!ValidateCustomer(firstname, surname, address, hasLicense))
                throw new InvalidOperationException("Please correct the required customer information.");

            if (carcombobox.SelectedItem == null)
            {
                MessageBox.Show("Please select a car type.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                carcombobox.Focus();
                throw new InvalidOperationException("Car type is required.");
            }

            if (fuelcombobox.SelectedItem == null)
            {
                MessageBox.Show("Please select a fuel type.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                fuelcombobox.Focus();
                throw new InvalidOperationException("Fuel type is required.");
            }

            CarType carType = ToCarType(carcombobox.Text);
            FuelType fuelType = ToFuelType(fuelcombobox.Text);

            Car selectedCar = repository.GetAvailableCar(carType, fuelType);

            if (selectedCar == null)
            {
                throw new InvalidOperationException(
                    "No available vehicle matches the selected car type and fuel type.");
            }

            Booking booking = new Booking
            {
                CustomerFirstName = firstname,
                CustomerSurname = surname,
                CustomerAddress = address,
                CustomerAge = (int)ageupdown.Value,
                HasValidLicense = hasLicense,
                NumberOfDays = (int)daysupdown.Value,
                SelectedCar = selectedCar,
                CarId = selectedCar.CarId,
                Extras = new OptionalExtras
                {
                    UnlimitedMileage = umcheckbox.Checked,
                    BreakdownCover = bccheckbox.Checked
                }
            };

            return booking;
        }

        private bool ValidateCustomer(
            string firstname,
            string surname,
            string address,
            bool hasLicense)
        {
            if (string.IsNullOrWhiteSpace(firstname))
            {
                MessageBox.Show("First name is required.", "Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                firstnamebox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(surname))
            {
                MessageBox.Show("Surname is required.", "Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lastnamebox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Address is required.", "Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                addressbox.Focus();
                return false;
            }

            if (!hasLicense)
            {
                MessageBox.Show("A valid driving licence is required.",
                    "Licence Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if ((int)ageupdown.Value < 18)
            {
                MessageBox.Show("Customer age must be at least 18.",
                    "Invalid Age", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if ((int)daysupdown.Value < 1 || (int)daysupdown.Value > 28)
            {
                MessageBox.Show("Rental days must be between 1 and 28.",
                    "Invalid Rental Days", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private CarType ToCarType(string value)
        {
            switch (value.Trim().ToLowerInvariant())
            {
                case "family car": return CarType.FamilyCar;
                case "sports car": return CarType.SportsCar;
                case "suv": return CarType.SUV;
                default: return CarType.CityCar;
            }
        }

        private FuelType ToFuelType(string value)
        {
            switch (value.Trim().ToLowerInvariant())
            {
                case "diesel": return FuelType.Diesel;
                case "hybrid": return FuelType.Hybrid;
                case "full electric":
                case "electric": return FuelType.FullElectric;
                default: return FuelType.Petrol;
            }
        }

        private void ShowBill(Booking booking)
        {
            string extras = "None";

            if (booking.Extras != null)
            {
                if (booking.Extras.UnlimitedMileage && booking.Extras.BreakdownCover)
                    extras = "Unlimited Mileage, Breakdown Cover";
                else if (booking.Extras.UnlimitedMileage)
                    extras = "Unlimited Mileage";
                else if (booking.Extras.BreakdownCover)
                    extras = "Breakdown Cover";
            }

            MessageBox.Show(
                "----- RENTAL BILL -----\n\n" +
                "Customer Name : " + booking.CustomerFirstName + " " + booking.CustomerSurname + "\n" +
                "Address       : " + booking.CustomerAddress + "\n" +
                "Age           : " + booking.CustomerAge + "\n" +
                "Licence       : Valid\n" +
                "Vehicle       : " + booking.SelectedCar.RegistrationNumber + "\n" +
                "Car Type      : " + booking.SelectedCar.CarType + "\n" +
                "Fuel Type     : " + booking.SelectedCar.FuelType + "\n" +
                "Days          : " + booking.NumberOfDays + "\n" +
                "Extras        : " + extras + "\n\n" +
                "Total Cost    : £" + booking.TotalCost.ToString("0.00") +
                "\n------------------------",
                "Booking Bill",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void ClearForm()
        {
            firstnamebox.Clear();
            lastnamebox.Clear();
            addressbox.Clear();
            ageupdown.Value = Math.Max(ageupdown.Minimum, 18);
            daysupdown.Value = 1;
            yesradiobutton.Checked = false;
            noradiobutton.Checked = true;
            carcombobox.SelectedIndex = -1;
            fuelcombobox.SelectedIndex = -1;
            umcheckbox.Checked = false;
            bccheckbox.Checked = false;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (ageupdown.Value < 18)
                ageupdown.Value = 18;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (daysupdown.Value < 1)
                daysupdown.Value = 1;

            if (daysupdown.Value > 28)
                daysupdown.Value = 28;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
