using ServerApp.Server;
using System;
using System.IO;
using System.Net;
using System.Text.Json;

namespace ServerApp.Helpers
{
    internal static class ConfigInteractor
    {
        //TODO: Add asynchronous file access
        private const string CONFIG_PATH = @".\Data";
        private const string CONFIG_FILE_NAME = @"config.json";
        private const int DEFAULT_PORT = 50000;
        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true
        };

        public static Config LoadConfig()
        {
            if (!Directory.Exists(CONFIG_PATH)) Directory.CreateDirectory(CONFIG_PATH);
            if (!File.Exists(Path.Combine(CONFIG_PATH, CONFIG_FILE_NAME)))
            {
                File.Create(Path.Combine(CONFIG_PATH, CONFIG_FILE_NAME)).Close();
                Config defaultConfig = new("localhost", DEFAULT_PORT);
                string jsonData = JsonSerializer.Serialize<Config>(defaultConfig, jsonSerializerOptions);
                File.WriteAllText(Path.Combine(CONFIG_PATH, CONFIG_FILE_NAME), jsonData);
            }
            string data = File.ReadAllText(Path.Combine(CONFIG_PATH, CONFIG_FILE_NAME));
            Config config = JsonSerializer.Deserialize<Config>(data, jsonSerializerOptions);
            if (!IPAddress.TryParse(config.IP, out IPAddress ip))
            {
                try
                {
                    ip = Dns.GetHostEntry(config.IP).AddressList[0];
                }
                catch (Exception)
                {
                    Logger.Write($"Bad config IP: {config.IP}, set default \"localhost\"");
                    Config defaultConfig = new("localhost", DEFAULT_PORT);
                    string jsonData = JsonSerializer.Serialize<Config>(defaultConfig, jsonSerializerOptions);
                    File.WriteAllText(Path.Combine(CONFIG_PATH, CONFIG_FILE_NAME), jsonData);
                    ip = Dns.GetHostEntry(defaultConfig.IP).AddressList[0];
                }
            }
            return new Config(ip.ToString(), config.Port);
        }

        public static void SaveConfig(Config config)
        {
            if (!Directory.Exists(CONFIG_PATH)) Directory.CreateDirectory(CONFIG_PATH);
            if (!File.Exists(Path.Combine(CONFIG_PATH, CONFIG_FILE_NAME))) File.Create(Path.Combine(CONFIG_PATH, CONFIG_FILE_NAME)).Close();
            Config defaultConfig = new(config.IP, config.Port);
            string jsonData = JsonSerializer.Serialize<Config>(defaultConfig, jsonSerializerOptions);
            File.WriteAllText(Path.Combine(CONFIG_PATH, CONFIG_FILE_NAME), jsonData);
        }
    }
}
