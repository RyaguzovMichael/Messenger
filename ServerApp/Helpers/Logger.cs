using System;
using System.IO;

namespace ServerApp.Helpers
{
    internal static class Logger
    {
        private const string LOG_PATH = @"..\..\..\Data\log.txt";

        static Logger()
        {
            Write("-------------- New Session Start ---------------");
        }

        internal static void Write(string text)
        {
            string logString = $"> {DateTime.Now} : ";
            logString += text;
            logString += "\n";
            if (!File.Exists(LOG_PATH)) File.Create(LOG_PATH).Close();
            File.AppendAllText(LOG_PATH, logString);
        }
    }
}