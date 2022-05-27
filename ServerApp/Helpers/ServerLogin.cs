using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ServerApp.Helpers
{
    internal static class ServerLogin
    {
        //TODO: Add asynchronous file access
        private const string LOGIN_DATA_PATH = @".\Data";
        private const string LOGIN_DATA_FILE_NAME = @"loginData.json";
        private static Dictionary<string, string> _loginPasswordAviliablePairs;

        static ServerLogin()
        {
            _loginPasswordAviliablePairs = new Dictionary<string, string>();
            if (!Directory.Exists(LOGIN_DATA_PATH)) Directory.CreateDirectory(LOGIN_DATA_PATH);
            if (!File.Exists(Path.Combine(LOGIN_DATA_PATH, LOGIN_DATA_FILE_NAME))) File.Create(Path.Combine(LOGIN_DATA_PATH, LOGIN_DATA_FILE_NAME));
            string data = File.ReadAllText(Path.Combine(LOGIN_DATA_PATH, LOGIN_DATA_FILE_NAME));
            _loginPasswordAviliablePairs = JsonSerializer.Deserialize<Dictionary<string, string>>(data);
        }

        internal static Guid TryLogin(string login, string password)
        {
            if (_loginPasswordAviliablePairs.TryGetValue(login, out string savedPassword))
            {
                if (password == savedPassword)
                {
                    return Guid.NewGuid();
                }
                return Guid.Empty;
            }
            RegisterUser(login, password);
            return Guid.NewGuid();
        }

        private static void RegisterUser(string login, string password)
        {
            _loginPasswordAviliablePairs.Add(login, password);
            SaveData();
        }

        private static void SaveData()
        {
            string data = JsonSerializer.Serialize<Dictionary<string, string>>(_loginPasswordAviliablePairs);
            if (!Directory.Exists(LOGIN_DATA_PATH)) Directory.CreateDirectory(LOGIN_DATA_PATH);
            if (!File.Exists(Path.Combine(LOGIN_DATA_PATH, LOGIN_DATA_FILE_NAME))) File.Create(Path.Combine(LOGIN_DATA_PATH, LOGIN_DATA_FILE_NAME));
            File.WriteAllText(data, Path.Combine(LOGIN_DATA_PATH, LOGIN_DATA_FILE_NAME));
        }
    }
}