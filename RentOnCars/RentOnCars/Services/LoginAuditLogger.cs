using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace RentOnCars.Services
{
    /// <summary>
    /// Audit log entry representing an authentication event.
    /// </summary>
    public class LoginAuditEntry
    {
        public DateTime Timestamp { get; set; }
        public string Username { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] User: {Username} | Success: {Success} | {Message}";
        }
    }

    /// <summary>
    /// Implements the "Log successful login" step from the assignment login flowchart.
    /// Records login activities in memory and writes to a persistent diagnostic log.
    /// </summary>
    public static class LoginAuditLogger
    {
        private static readonly List<LoginAuditEntry> entries = new List<LoginAuditEntry>();
        private static readonly object syncLock = new object();
        private const string LogFileName = "login_audit.log";

        public static IReadOnlyList<LoginAuditEntry> Entries
        {
            get
            {
                lock (syncLock)
                {
                    return new List<LoginAuditEntry>(entries);
                }
            }
        }

        /// <summary>
        /// Logs a successful staff login.
        /// </summary>
        public static void LogSuccess(string username)
        {
            Log(username, true, "Staff authentication successful. Dashboard access granted.");
        }

        /// <summary>
        /// Logs a failed login attempt with reason.
        /// </summary>
        public static void LogFailure(string username, string reason)
        {
            Log(username, false, $"Login rejected: {reason}");
        }

        private static void Log(string username, bool success, string message)
        {
            LoginAuditEntry entry = new LoginAuditEntry
            {
                Timestamp = DateTime.Now,
                Username = string.IsNullOrWhiteSpace(username) ? "(empty)" : username,
                Success = success,
                Message = message
            };

            lock (syncLock)
            {
                entries.Add(entry);
            }

            string line = entry.ToString();
            Debug.WriteLine("[LOGIN AUDIT] " + line);

            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogFileName);
                File.AppendAllText(path, line + Environment.NewLine);
            }
            catch
            {
                // Never crash the application if disk log writing encounters permission/lock issues
            }
        }
    }
}
