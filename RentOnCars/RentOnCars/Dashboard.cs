using System;
using System.Drawing;
using System.Threading.Tasks;
using RentOnCars.Services;
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
        private bool loading;
        public event EventHandler LogoutRequested;

        public Dashboard()
        {
            InitializeComponent();
            ApplyCustomStyling();
            staffBadgeLabel.Text = StaffSession.Current?.Username ?? "Staff";
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
        public async void LoadDashboardMetrics()
        {
            if(loading||IsDisposed)return;
            loading=true; refreshButton.Enabled=false;
            try
            {
                var stats=await Task.Run(() => repository.GetDashboardStats());
                if(IsDisposed)return;
                isDatabaseAvailable=true;
                totalCarsValueLabel.Text=stats.TotalVehicles.ToString(); availableCarsValueLabel.Text=stats.AvailableVehicles.ToString();
                rentedCarsValueLabel.Text=stats.RentedVehicles.ToString(); totalBookingsValueLabel.Text=stats.TotalBookings.ToString();
                dbStatusPanel.BackColor=Theme.SuccessGreenBg;dbStatusLabel.ForeColor=Theme.SuccessGreen;
                dbStatusLabel.Text="Database connected ? Inventory is up to date";dbGuidanceLabel.Visible=false;retryDbButton.Visible=false;
                newBookingButton.Enabled=rentedCarsButton.Enabled=bookingHistoryButton.Enabled=true;
            }
            catch(Exception ex) {if(!IsDisposed)ShowDatabaseOfflineState(ex.Message);}
            finally {loading=false;if(!IsDisposed)refreshButton.Enabled=true;}
        }

        private void ShowDatabaseOfflineState(string errorMessage)
        {
            isDatabaseAvailable = false;
            rentedCarsButton.Enabled=bookingHistoryButton.Enabled=false;

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

            StaffSession.SignOut();
            LogoutRequested?.Invoke(this, EventArgs.Empty);
        }

        private void newBookingButton_Click(object sender, EventArgs e) => openNewBooking();

        private void rentedCarsButton_Click(object sender, EventArgs e) => displayRentedCarsList();

        private void bookingHistoryButton_Click(object sender, EventArgs e) => openBookingHistory();

        private void logoutButton_Click(object sender, EventArgs e) => logout();

        private void refreshButton_Click(object sender, EventArgs e) => LoadDashboardMetrics();

        private async void retryDbButton_Click(object sender, EventArgs e)
        {
            retryDbButton.Enabled=false;
            await Task.Run(() => DatabaseInitializer.Initialize());
            if(IsDisposed)return;
            retryDbButton.Enabled=true; LoadDashboardMetrics();
        }

        private void Dashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            // RentalApplicationContext owns application lifetime.
            if (Application.OpenForms.Count == 0)
            {
                Application.Exit();
            }
        }
    }
}
