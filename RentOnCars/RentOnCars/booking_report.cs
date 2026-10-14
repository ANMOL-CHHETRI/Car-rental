using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RentOnCars.Models;
using RentOnCars.Repos;
using RentOnCars.UI;

namespace RentOnCars
{
    /// <summary>
    /// Dual-mode fleet reporting screen:
    /// 1. Rented Cars Mode: Read-only active fleet grid with category, fuel, daily rate, and search.
    /// 2. Booking History Mode: Complete historical log with stored totals and saved optional extras detail view.
    /// </summary>
    public partial class booking_report : Form
    {
        private readonly bool showRentedCarsOnly;
        private readonly BookingRepository repository = new BookingRepository();
        private List<Booking> cachedBookings = new List<Booking>();
        private List<Car> cachedRentedCars = new List<Car>();

        public booking_report() : this(false)
        {
        }

        public booking_report(bool rentedCarsOnly)
        {
            showRentedCarsOnly = rentedCarsOnly;
            InitializeComponent();
            ApplyCustomStyling();
        }

        private void booking_report_Load(object sender, EventArgs e)
        {
            readData();
        }

        private void ApplyCustomStyling()
        {
            Theme.ApplyDataGrid(dataGridView1);
            Theme.ApplySecondaryButton(backButton);
            Theme.ApplySecondaryButton(refreshButton);

            if (showRentedCarsOnly)
            {
                Text = "WeAreCars - Currently Rented Vehicles";
                screenTitleLabel.Text = "Rented Fleet Inventory";
                screenSubtitleLabel.Text = "Active vehicle rentals, allocations, categories, and fuel types.";
                detailsGroup.Visible = false;
                gridPanel.Dock = DockStyle.Fill;
            }
            else
            {
                Text = "WeAreCars - Booking History & Details";
                screenTitleLabel.Text = "Booking History & Audit";
                screenSubtitleLabel.Text = "Complete transaction records with stored totals and saved optional extras.";
                detailsGroup.Visible = true;
            }
        }

        /// <summary>
        /// Reads records from database repository and binds them to the styled DataGridView.
        /// Handles empty states and error recovery gracefully.
        /// </summary>
        public void readData()
        {
            try
            {
                emptyStateLabel.Visible = false;

                if (showRentedCarsOnly)
                {
                    LoadRentedCarsData();
                }
                else
                {
                    LoadBookingHistoryData();
                }
            }
            catch (Exception ex)
            {
                emptyStateLabel.Text = "Unable to load database records:\n" + ex.Message;
                emptyStateLabel.ForeColor = Theme.DangerRed;
                emptyStateLabel.Visible = true;
            }
        }

        private void LoadRentedCarsData()
        {
            cachedRentedCars = repository.GetRentedCars();
            ApplyRentedCarsFilter();
        }

        private void ApplyRentedCarsFilter()
        {
            string search = searchBox.Text.Trim().ToLowerInvariant();

            DataTable dt = new DataTable();
            dt.Columns.Add("Car ID", typeof(int));
            dt.Columns.Add("Registration Number", typeof(string));
            dt.Columns.Add("Vehicle Category", typeof(string));
            dt.Columns.Add("Fuel Type", typeof(string));
            dt.Columns.Add("Base Daily Rate", typeof(string));
            dt.Columns.Add("Rental Status", typeof(string));

            int matchCount = 0;
            foreach (Car car in cachedRentedCars)
            {
                if (!string.IsNullOrEmpty(search))
                {
                    bool matches = car.RegistrationNumber.ToLowerInvariant().Contains(search) ||
                                   car.CarType.ToString().ToLowerInvariant().Contains(search) ||
                                   car.FuelType.ToString().ToLowerInvariant().Contains(search);
                    if (!matches)
                        continue;
                }

                DataRow row = dt.NewRow();
                row["Car ID"] = car.CarId;
                row["Registration Number"] = car.RegistrationNumber;
                row["Vehicle Category"] = car.CarType.ToString();
                row["Fuel Type"] = car.FuelType.ToString();
                row["Base Daily Rate"] = $"£{car.DailyRate:0.00}";
                row["Rental Status"] = "Hired / Rented";
                dt.Rows.Add(row);
                matchCount++;
            }

            dataGridView1.DataSource = dt;
            recordCountLabel.Text = $"Showing {matchCount} of {cachedRentedCars.Count} rented vehicle(s)";

            if (matchCount == 0)
            {
                emptyStateLabel.Text = cachedRentedCars.Count == 0
                    ? "No vehicles are currently rented. Fleet is fully available."
                    : "No rented vehicles match your search filter.";
                emptyStateLabel.ForeColor = Theme.TextMuted;
                emptyStateLabel.Visible = true;
            }
            else
            {
                emptyStateLabel.Visible = false;
            }
        }

        private void LoadBookingHistoryData()
        {
            cachedBookings = repository.GetDetails();
            ApplyBookingHistoryFilter();
        }

        private void ApplyBookingHistoryFilter()
        {
            string search = searchBox.Text.Trim().ToLowerInvariant();

            DataTable dt = new DataTable();
            dt.Columns.Add("Booking ID", typeof(string));
            dt.Columns.Add("Created Date", typeof(string));
            dt.Columns.Add("Customer Name", typeof(string));
            dt.Columns.Add("Registration", typeof(string));
            dt.Columns.Add("Vehicle Category", typeof(string));
            dt.Columns.Add("Fuel Type", typeof(string));
            dt.Columns.Add("Rental Days", typeof(int));
            dt.Columns.Add("Stored Total Cost", typeof(string));

            int matchCount = 0;
            foreach (Booking booking in cachedBookings)
            {
                string customerName = $"{booking.CustomerFirstName} {booking.CustomerSurname}";
                string bookingRef = $"WAC-BK-{booking.BookingId:D5}";

                if (!string.IsNullOrEmpty(search))
                {
                    bool matches = customerName.ToLowerInvariant().Contains(search) ||
                                   bookingRef.ToLowerInvariant().Contains(search) ||
                                   booking.SelectedCar.RegistrationNumber.ToLowerInvariant().Contains(search) ||
                                   booking.CustomerAddress.ToLowerInvariant().Contains(search);
                    if (!matches)
                        continue;
                }

                DataRow row = dt.NewRow();
                row["Booking ID"] = bookingRef;
                row["Created Date"] = booking.CreatedAt.ToString("yyyy-MM-dd HH:mm");
                row["Customer Name"] = customerName;
                row["Registration"] = booking.SelectedCar != null ? booking.SelectedCar.RegistrationNumber : "--";
                row["Vehicle Category"] = booking.SelectedCar != null ? booking.SelectedCar.CarType.ToString() : "--";
                row["Fuel Type"] = booking.SelectedCar != null ? booking.SelectedCar.FuelType.ToString() : "--";
                row["Rental Days"] = booking.NumberOfDays;
                row["Stored Total Cost"] = $"£{booking.TotalCost:0.00}";
                dt.Rows.Add(row);
                matchCount++;
            }

            dataGridView1.DataSource = dt;
            recordCountLabel.Text = $"Showing {matchCount} of {cachedBookings.Count} booking(s)";

            if (matchCount == 0)
            {
                emptyStateLabel.Text = cachedBookings.Count == 0
                    ? "No bookings have been registered yet."
                    : "No bookings match your search query.";
                emptyStateLabel.ForeColor = Theme.TextMuted;
                emptyStateLabel.Visible = true;
                ClearDetailPanel();
            }
            else
            {
                emptyStateLabel.Visible = false;
                if (dataGridView1.Rows.Count > 0)
                {
                    DisplayBookingDetails(0);
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (showRentedCarsOnly || dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.Index < 0)
                return;

            DisplayBookingDetails(dataGridView1.CurrentRow.Index);
        }

        private void DisplayBookingDetails(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= cachedBookings.Count)
            {
                ClearDetailPanel();
                return;
            }

            Booking b = cachedBookings[rowIndex];
            detailBookingRefLabel.Text = $"Booking Reference: WAC-BK-{b.BookingId:D5}";
            detailCustomerNameLabel.Text = $"Customer: {b.CustomerFirstName} {b.CustomerSurname} (Age: {b.CustomerAge})";
            detailAddressLabel.Text = $"Address: {b.CustomerAddress}";
            detailLicenseLabel.Text = $"Licence: {(b.HasValidLicense ? "Valid Driving Licence Verified" : "Invalid Licence")}";
            detailVehicleLabel.Text = $"Vehicle: {b.SelectedCar.RegistrationNumber} ({b.SelectedCar.CarType} - {b.SelectedCar.FuelType})";
            detailDaysLabel.Text = $"Rental Duration: {b.NumberOfDays} Days";

            // Saved Optional Extras retrieval demonstration
            string extras = b.Extras != null ? b.Extras.GetDescription() : "None";
            detailExtrasLabel.Text = $"Saved Optional Extras: {extras}";

            detailStoredTotalLabel.Text = $"Stored Total: £{b.TotalCost:0.00} GBP (Recorded at booking time)";
        }

        private void ClearDetailPanel()
        {
            detailBookingRefLabel.Text = "Select a booking above to view complete details.";
            detailCustomerNameLabel.Text = string.Empty;
            detailAddressLabel.Text = string.Empty;
            detailLicenseLabel.Text = string.Empty;
            detailVehicleLabel.Text = string.Empty;
            detailDaysLabel.Text = string.Empty;
            detailExtrasLabel.Text = string.Empty;
            detailStoredTotalLabel.Text = string.Empty;
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            if (showRentedCarsOnly)
                ApplyRentedCarsFilter();
            else
                ApplyBookingHistoryFilter();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            readData();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
