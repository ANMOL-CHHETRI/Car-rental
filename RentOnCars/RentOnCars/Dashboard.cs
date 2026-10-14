using System;
using System.Windows.Forms;

namespace RentOnCars
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        public void displayRentedCarsList()
        {
            booking_report reportForm = new booking_report(true);
            reportForm.ShowDialog(this);
        }

        public void openNewBooking()
        {
            Details detailsForm = new Details();
            detailsForm.ShowDialog(this);
        }

        public void logout()
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openNewBooking();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            displayRentedCarsList();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            logout();
        }
    }
}
