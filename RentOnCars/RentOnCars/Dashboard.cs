using System;
using System.Drawing;
using System.Windows.Forms;
using RentOnCars.Repos;
using RentOnCars.UI;

namespace RentOnCars
{
    /// <summary>
    /// Central operational hub for WeAreCars staff.
    /// Displays live fleet metrics, database status, and routes to booking and reporting screens.
    /// </summary>
    public partial class Dashboard : Form
    {
        private readonly BookingRepository repository = new BookingRepository();
        private bool isDatabaseAvailable = false;

        public Dashboard()
        {
            InitializeComponent();
            ApplyCustomStyling();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            LoadDashboardMetrics();
        }

        private void ApplyCustomStyling()
        {
            Theme.ApplyPrimaryButton(newBookingButton);
            Theme.ApplySecondaryButton(rentedCarsButton);
            Theme.ApplySecondaryButton(bookingHistoryButton);
            Theme.ApplySecondaryButton(retryDbButton);
            Theme.ApplySecondaryButton(refreshButton);
        }

        /// <summary>
        /// Loads fleet metrics and total bookings from the database.
        /// Displays clear empty and unavailable states when the database is unreachable.
        /// </summary>
        public void LoadDashboardMetrics()
        {
            try
            {
                string error;
                if (!repository.TestConnection(out error))
                {
                    ShowDatabaseOfflineState(error);
                    return;
                }

                DashboardStats stats = repository.GetDashboardStats();
                isDatabaseAvailable = true;

                // Update KPI Cards
                totalCarsValueLabel.Text = stats.TotalVehicles.ToString();
                availableCarsValueLabel.Text = stats.AvailableVehicles.ToString();
                rentedCarsValueLabel.Text = stats.RentedVehicles.ToString();
                totalBookingsValueLabel.Text = stats.TotalBookings.ToString();

                // Status Banner
                dbStatusPanel.BackColor = Theme.SuccessGreenBg;
                dbStatusLabel.ForeColor = Theme.SuccessGreen;
                dbStatusLabel.Text = "● Database Online: Connected to SQL Server LocalDB (RentOnCarsDB)";
                dbGuidanceLabel.Visible = false;
                retryDbButton.Visible = false;

                // Enable database-dependent actions
                newBookingButton.Enabled = true;
                rentedCarsButton.Enabled = true;
                bookingHistoryButton.Enabled = true;
            }
            catch (Exception ex)
            {
                ShowDatabaseOfflineState(ex.Message);
            }
        }

        private void ShowDatabaseOfflineState(string errorMessage)
        {
            isDatabaseAvailable = false;

            totalCarsValueLabel.Text = "--";
            availableCarsValueLabel.Text = "--";
            rentedCarsValueLabel.Text = "--";
            totalBookingsValueLabel.Text = "--";

            dbStatusPanel.BackColor = Theme.WarningAmberBg;
            dbStatusLabel.ForeColor = Theme.WarningAmber;
            dbStatusLabel.Text = "▲ Database Offline: SQL Server LocalDB could not be contacted.";

            dbGuidanceLabel.Visible = true;
            dbGuidanceLabel.Text = "Database-dependent actions are paused. Verify SqlLocalDB instance or start MSSQLLocalDB.\n" +
                                  (errorMessage ?? string.Empty);
            retryDbButton.Visible = true;
        }

        /// <summary>
        /// UML Class Diagram operation: + displayRentedCarsList() : void
        /// Opens the read-only list of currently rented vehicles.
        /// </summary>
        public void displayRentedCarsList()
        {
            if (!isDatabaseAvailable)
            {
                MessageBox.Show(
                    "Database is currently unavailable. Please verify LocalDB connection.",
                    "Database Offline",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            using (booking_report reportForm = new booking_report(true))
            {
                reportForm.ShowDialog(this);
            }
            LoadDashboardMetrics();
        }

        /// <summary>
        /// UML Class Diagram operation: + openNewBooking() : void
        /// Opens the new booking screen for registering customer rental details.
        /// </summary>
        public void openNewBooking()
        {
            using (Details detailsForm = new Details())
            {
                detailsForm.ShowDialog(this);
            }
            LoadDashboardMetrics();
        }

        /// <summary>
        /// Opens the full booking history view with saved optional extras and customer details.
        /// </summary>
        public void openBookingHistory()
        {
            if (!isDatabaseAvailable)
            {
                MessageBox.Show(
                    "Database is currently unavailable. Please verify LocalDB connection.",
                    "Database Offline",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            using (booking_report reportForm = new booking_report(false))
            {
                reportForm.ShowDialog(this);
            }
            LoadDashboardMetrics();
        }

        /// <summary>
        /// UML Class Diagram operation: + logout() : void
        /// Logs out the staff member and returns cleanly to the login screen.
        /// </summary>
        public void logout()
        {
            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to log out of the WeAreCars staff portal?",
                "WeAreCars - Logout Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void newBookingButton_Click(object sender, EventArgs e) => openNewBooking();

        private void rentedCarsButton_Click(object sender, EventArgs e) => displayRentedCarsList();

        private void bookingHistoryButton_Click(object sender, EventArgs e) => openBookingHistory();

        private void logoutButton_Click(object sender, EventArgs e) => logout();

        private void refreshButton_Click(object sender, EventArgs e) => LoadDashboardMetrics();

        private void retryDbButton_Click(object sender, EventArgs e)
        {
            DatabaseInitializer.Initialize();
            LoadDashboardMetrics();
        }

        private void Dashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            // If all forms are closed, ensure application terminates cleanly
            if (Application.OpenForms.Count == 0)
            {
                Application.Exit();
            }
        }
    }
}
