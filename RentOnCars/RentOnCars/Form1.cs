using System;
using System.Drawing;
using System.Windows.Forms;
using RentOnCars.Services;
using RentOnCars.UI;

namespace RentOnCars
{
    /// <summary>
    /// Staff authentication screen for WeAreCars.
    /// Strictly implements the credential validation flowchart and class diagram members:
    /// - Required staff username: sta001
    /// - Required staff password: givemethekeys123
    /// - Masks password with accessible show/hide toggle
    /// - Enter-key submission
    /// - Flowchart feedback messages
    /// - Logs successful login
    /// </summary>
    public partial class LoginForm : Form
    {
        public const string StaffUsername = "sta001";
        public const string StaffPassword = "givemethekeys123";

        // Properties matching the UML Class Diagram: username : string, password : string
        public string username => usernameBox != null ? usernameBox.Text.Trim() : string.Empty;
        public string password => passwordBox != null ? passwordBox.Text : string.Empty;

        public LoginForm()
        {
            InitializeComponent();
            ApplyCustomStyling();
        }

        private void ApplyCustomStyling()
        {
            AcceptButton = loginButton;
            passwordBox.UseSystemPasswordChar = true;
            Theme.ApplyPrimaryButton(loginButton);
        }

        /// <summary>
        /// Validates entered credentials against the required staff credentials.
        /// Implements the validateCredentials() method from the UML class diagram.
        /// </summary>
        public bool validateCredentials()
        {
            return ValidateCredentials(username, password);
        }

        /// <summary>
        /// Validates provided credentials against the authoritative staff assignment requirements.
        /// </summary>
        public bool ValidateCredentials(string u, string p)
        {
            return string.Equals(u, StaffUsername, StringComparison.Ordinal) &&
                   string.Equals(p, StaffPassword, StringComparison.Ordinal);
        }

        /// <summary>
        /// Executes the login validation process strictly following the assignment flowchart:
        /// 1. Check if both fields are filled.
        /// 2. Check if username matches "sta001".
        /// 3. Check if password matches "givemethekeys123".
        /// 4. Log successful login and load Dashboard.
        /// </summary>
        public bool login()
        {
            string u = username;
            string p = password;

            // Decision: Are Username and Password fields both filled?
            if (string.IsNullOrWhiteSpace(u) || string.IsNullOrWhiteSpace(p))
            {
                LoginAuditLogger.LogFailure(u, "Missing username or password fields.");
                MessageBox.Show(
                    "Please enter username and password",
                    "WeAreCars - Login",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                if (string.IsNullOrWhiteSpace(u))
                    usernameBox.Focus();
                else
                    passwordBox.Focus();

                return false;
            }

            // Decision: Does Username match "sta001"?
            if (!string.Equals(u, StaffUsername, StringComparison.Ordinal))
            {
                LoginAuditLogger.LogFailure(u, "Invalid username.");
                MessageBox.Show(
                    "Invalid username",
                    "WeAreCars - Login",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                usernameBox.SelectAll();
                usernameBox.Focus();
                return false;
            }

            // Decision: Does Password match "givemethekeys123"?
            if (!string.Equals(p, StaffPassword, StringComparison.Ordinal))
            {
                LoginAuditLogger.LogFailure(u, "Invalid password.");
                MessageBox.Show(
                    "Invalid password",
                    "WeAreCars - Login",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                passwordBox.Clear();
                passwordBox.Focus();
                return false;
            }

            // "Log successful login" from flowchart
            LoginAuditLogger.LogSuccess(u);
            return true;
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            if (!login())
                return;

            // Grant access and load Dashboard
            Dashboard dashboard = new Dashboard();
            dashboard.FormClosed += (s, args) =>
            {
                // Ensure application exits if Dashboard is closed directly
                if (Application.OpenForms.Count == 0 ||
                    (Application.OpenForms.Count == 1 && Application.OpenForms[0] == this))
                {
                    Application.Exit();
                }
            };

            dashboard.Show();
            this.Hide();
        }

        private void showPasswordCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            passwordBox.UseSystemPasswordChar = !showPasswordCheckBox.Checked;
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If user closes login form, exit application
            if (e.CloseReason == CloseReason.UserClosing && !Visible)
            {
                // Was hidden because dashboard opened; allow close
            }
            else if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }
    }
}
