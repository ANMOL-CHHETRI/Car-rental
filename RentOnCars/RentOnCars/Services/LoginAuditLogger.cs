using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
        public static string LastWriteError { get; private set; }
        private static readonly string logDirectory=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"WeAreCars","logs");

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
                Timestamp = DateTime.UtcNow,
                Username = string.IsNullOrWhiteSpace(username) ? "(empty)" : Regex.Replace(username,@"[\r\n\t]"," "),
                Success = success,
                Message = Regex.Replace(message,@"[\r\n\t]"," ")
            };

            lock (syncLock)
            {
                entries.Add(entry);
                if(entries.Count>1000)entries.RemoveRange(0,entries.Count-1000);
                string line=entry.ToString();Debug.WriteLine("[LOGIN AUDIT] "+line);
                try
                {
                    Directory.CreateDirectory(logDirectory);
                    string path=Path.Combine(logDirectory,LogFileName);
                    if(File.Exists(path)&&new FileInfo(path).Length>1024*1024)
                    {
                        File.Move(path,Path.Combine(logDirectory,"login_audit."+DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")+".log"));
                        foreach(string old in Directory.GetFiles(logDirectory,"login_audit.*.log").OrderByDescending(File.GetLastWriteTimeUtc).Skip(4))File.Delete(old);
                    }
                    File.AppendAllText(path,line+Environment.NewLine);LastWriteError=null;
                }
                catch(Exception ex) when(ex is IOException||ex is UnauthorizedAccessException)
                {
                    LastWriteError=ex.Message;
                    Trace.TraceError("Diagnostic login log could not be written: "+ex.Message);
                    // Authoritative authentication and transaction audit events are committed in SQL.
                }
            }
        }
    }
}
