using System;
using System.Diagnostics;
using System.Windows.Forms;
using RentOnCars.UI;

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
            string logFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WeAreCars", "startup.log");
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logFile));
            Action<string> log = (msg) =>
            {
                try { System.IO.File.AppendAllText(logFile, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {msg}\r\n"); } catch { }
            };

            log("Application Main started.");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += (s, e) =>
            {
                log($"ThreadException: {e.Exception}");
                MessageBox.Show($"Startup Error: {e.Exception.Message}", "WeAreCars Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                log($"UnhandledException: {e.ExceptionObject}");
            };

            // Launch WeAreCars application via Welcome splash
            try
            {
                log("Launching RentalApplicationContext...");
                Application.Run(new RentalApplicationContext());
                log("Application.Run exited normally.");
            }
            catch (Exception ex)
            {
                log($"Application.Run crashed with exception: {ex}");
            }
        }
    }
}
