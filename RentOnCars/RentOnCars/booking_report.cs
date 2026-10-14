using System;
using System.Data;
using System.Windows.Forms;
using RentOnCars.Repos;

namespace RentOnCars
{
    public partial class booking_report : Form
    {
        private readonly bool showRentedCarsOnly;
        private readonly Booking_repos repository = new Booking_repos();

        public booking_report() : this(false)
        {
        }

        public booking_report(bool rentedCarsOnly)
        {
            showRentedCarsOnly = rentedCarsOnly;
            InitializeComponent();
        }

        public void readData()
        {
            if (showRentedCarsOnly)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Car ID");
                dt.Columns.Add("Registration");
                dt.Columns.Add("Car Type");
                dt.Columns.Add("Fuel Type");
                dt.Columns.Add("Daily Rate");
                dt.Columns.Add("Status");

                foreach (var car in repository.GetRentedCars())
                {
                    DataRow row = dt.NewRow();
                    row["Car ID"] = car.CarId;
                    row["Registration"] = car.RegistrationNumber;
                    row["Car Type"] = car.CarType;
                    row["Fuel Type"] = car.FuelType;
                    row["Daily Rate"] = "£" + car.DailyRate.ToString("0.00");
                    row["Status"] = "Rented";
                    dt.Rows.Add(row);
                }

                dataGridView1.DataSource = dt;
                Text = "Rented Cars List";
            }
            else
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Booking ID");
                dt.Columns.Add("Customer");
                dt.Columns.Add("Address");
                dt.Columns.Add("Age");
                dt.Columns.Add("Licence");
                dt.Columns.Add("Days");
                dt.Columns.Add("Registration");
                dt.Columns.Add("Car Type");
                dt.Columns.Add("Fuel Type");
                dt.Columns.Add("Total Cost");

                foreach (var booking in repository.GetDetails())
                {
                    DataRow row = dt.NewRow();
                    row["Booking ID"] = booking.BookingId;
                    row["Customer"] = booking.CustomerFirstName + " " + booking.CustomerSurname;
                    row["Address"] = booking.CustomerAddress;
                    row["Age"] = booking.CustomerAge;
                    row["Licence"] = booking.HasValidLicense ? "Valid" : "Invalid";
                    row["Days"] = booking.NumberOfDays;
                    row["Registration"] = booking.SelectedCar.RegistrationNumber;
                    row["Car Type"] = booking.SelectedCar.CarType;
                    row["Fuel Type"] = booking.SelectedCar.FuelType;
                    row["Total Cost"] = "£" + booking.TotalCost.ToString("0.00");
                    dt.Rows.Add(row);
                }

                dataGridView1.DataSource = dt;
                Text = "Booking Report";
            }

            dataGridView1.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void booking_report_Load(object sender, EventArgs e)
        {
            try
            {
                readData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load database records.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
