using System;
using System.Linq;
using System.Windows.Forms;
using RentOnCars.Services;

namespace RentOnCars.UI
{
    public sealed class RentalApplicationContext : ApplicationContext
    {
        private readonly Timer sessionTimer = new Timer { Interval = 30000 };
        public RentalApplicationContext()
        {
            ShowLogin();
            sessionTimer.Tick += (s, e) => { if (MainForm is Dashboard && StaffSession.Current == null) ShowLogin(); };
            sessionTimer.Start();
        }
        private void ReplaceMain(Form next)
        {
            Form previous = MainForm;
            MainForm = next;
            if (previous != null)
            {
                foreach (Form child in Application.OpenForms.Cast<Form>().Where(f => f != previous && f != next).ToArray()) child.Close();
                previous.Close(); previous.Dispose();
            }
            next.Show();
        }
        private void ShowLogin()
        {
            StaffSession.SignOut();
            var login = new LoginForm();
            login.Authenticated += (s, e) =>
            {
                var dashboard = new Dashboard();
                dashboard.LogoutRequested += (sender, args) => ShowLogin();
                ReplaceMain(dashboard);
            };
            ReplaceMain(login);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) { sessionTimer.Stop(); sessionTimer.Dispose(); StaffSession.SignOut(); }
            base.Dispose(disposing);
        }
    }
}
