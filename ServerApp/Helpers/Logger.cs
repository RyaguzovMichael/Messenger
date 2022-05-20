using System;
using System.IO;

namespace ServerApp.Helpers;

internal static class Logger
{
    private const string LOG_PATH = @".\Data";
    private const string LOG_FILE_NAME = @"log.txt";

    static Logger()
    {
        Write("-------------- New Session Start ---------------");
    }

    internal static void Write(string text)
    {
        string logString = $"> {DateTime.Now} : ";
        logString += text;
        logString += "\n";
        if (!Directory.Exists(LOG_PATH)) Directory.CreateDirectory(LOG_PATH);
        if (!File.Exists(Path.Combine(LOG_PATH, LOG_FILE_NAME))) File.Create(Path.Combine(LOG_PATH, LOG_FILE_NAME)).Close();
        File.AppendAllText(Path.Combine(LOG_PATH, LOG_FILE_NAME), logString);
    }
}