using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZombiesAutosplitter
{
    public static class Logger
    {
        private static List<LogMessage> MessagesToLog { get; set; } = new List<LogMessage>();

        public static void Log(string message, LogType type = LogType.INFO)
        {
            MessagesToLog.Add(new LogMessage(message + Environment.NewLine, type));
        }

        public static void AppendLogs(RichTextBox logTextbox)
        {
            lock (MessagesToLog) {
                for (int i = 0; i < MessagesToLog.Count; i++)
                    logTextbox.AppendText(
                        MessagesToLog[i].LogType.ToString()
                        + " : " + MessagesToLog[i].Message);

                MessagesToLog.Clear();
            }
        }

        private struct LogMessage
        {
            public string Message { get; set; }
            public LogType LogType { get; set; }

            public LogMessage(string message, LogType logType)
            {
                this.Message = message;
                this.LogType = logType;
            }
        }
    }

    public enum LogType
    {
        INFO,
        WARNING,
        ERROR,
    }
}
