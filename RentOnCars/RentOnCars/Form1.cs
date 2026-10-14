using System;
using System.Windows.Forms;

namespace RentOnCars
{
    public partial class LoginForm : Form
    {
        private const string StaffUsername = "sta001";
        private const string StaffPassword = "givemethekeys123";

        public LoginForm()
        {
            InitializeComponent();
        }

        public bool ValidateCredentials(string username, string password)
        {
            return username == StaffUsername && password == StaffPassword;
        }

        public bool login()
        {
            string username = usernameBox.Text.Trim();
            string password = passwordBox.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Please enter both username and password.",
                    "Empty Fields",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            if (!ValidateCredentials(username, password))
            {
                MessageBox.Show(
                    "Invalid username or password.",
                    "Login Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                passwordBox.Clear();
                passwordBox.Focus();
                return false;
            }

            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!login())
                return;

            MessageBox.Show(
                "Login successful.",
                "Login Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            Dashboard dashboard = new Dashboard();
            dashboard.FormClosed += delegate { this.Close(); };
            dashboard.Show();
            this.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
