using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO1ZombiesAutosplitter
{
    public static class Logger
    {
        public static void Log(string message, LogType type)
        {
            string date = "[" + DateTime.Now.ToLongTimeString() + "] ";
            Console.WriteLine(date + type.ToString() + " : " + message);
        }
    }

    public enum LogType
    {
        SUCCESS,
        INFO,
        WARNING,
        ERROR,
    }
}
