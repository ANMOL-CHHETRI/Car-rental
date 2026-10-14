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
            string logFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startup.log");
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

            // Attempt automatic database preparation
            try
            {
                log("Calling DatabaseInitializer.Initialize()...");
                DatabaseInitResult initResult = DatabaseInitializer.Initialize();
                log($"DatabaseInitializer result: Success={initResult.IsSuccessful}, Message={initResult.Message}");
            }
            catch (Exception ex)
            {
                log($"Unexpected database init exception: {ex}");
            }

            // Launch WeAreCars application via Welcome splash
            try
            {
                log("Launching Application.Run(new Welcome_page())...");
                Application.Run(new Welcome_page());
                log("Application.Run exited normally.");
            }
            catch (Exception ex)
            {
                log($"Application.Run crashed with exception: {ex}");
            }
        }
    }
}
