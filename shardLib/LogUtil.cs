using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shardLib
{
    public static class LogUtil
    {
        private static readonly string logPath = Configer.LogPath;

        public static void Log(params object[] messages)
        {
            string logMessage = string.Join(" ", messages);
            string logLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {logMessage}";

            try
            {
                File.AppendAllText(logPath, logLine + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }
    }
}
