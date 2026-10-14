using System;
using System.Windows.Forms;

namespace RentOnCars
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                DatabaseInitializer.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "The database could not be prepared automatically.\n\n" +
                    ex.Message +
                    "\n\nThe login screen will still open. If the database is unavailable, booking/report features will show the database error.",
                    "RentOnCars Database Notice",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            Application.Run(new Welcome_page());
        }
    }
}
