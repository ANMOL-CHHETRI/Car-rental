using System;
using System.Drawing;
using System.Windows.Forms;
using RentOnCars.UI;

namespace RentOnCars
{
    /// <summary>
    /// Initial branded splash/welcome screen for WeAreCars car rental system.
    /// Manages window lifetime correctly to prevent duplicate login instances.
    /// </summary>
    public partial class Welcome_page : Form
    {
        private LoginForm activeLoginForm;

        public Welcome_page()
        {
            InitializeComponent();
            Theme.ApplyPrimaryButton(Startbutton);
        }

        private void Startbutton_Click(object sender, EventArgs e)
        {
            if (activeLoginForm != null && !activeLoginForm.IsDisposed)
            {
                activeLoginForm.BringToFront();
                activeLoginForm.Focus();
                return;
            }

            activeLoginForm = new LoginForm();
            activeLoginForm.FormClosed += (s, args) =>
            {
                // If the application is still running and login was simply closed, close welcome page to exit
                if (Application.OpenForms.Count == 0 ||
                    (Application.OpenForms.Count == 1 && Application.OpenForms[0] == this))
                {
                    this.Close();
                }
            };

            this.Hide();
            activeLoginForm.Show();
        }
    }
}
