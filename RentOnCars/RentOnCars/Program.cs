using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace RentOnCars
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Prepares database non-destructively and manages application startup.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Attempt automatic database preparation
            try
            {
                DatabaseInitResult initResult = DatabaseInitializer.Initialize();
                if (!initResult.IsSuccessful)
                {
                    Debug.WriteLine($"[Program] Database initialization note: {initResult.Message}");
                    // Do not block app launch with a crash; Dashboard and reports handle offline state with retry actions.
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Program] Unexpected database init exception: {ex.Message}");
            }

            // Launch WeAreCars application via Welcome splash
            Application.Run(new Welcome_page());
        }
    }
}
