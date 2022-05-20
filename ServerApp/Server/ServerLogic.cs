using ServerApp.Interfaces;
using ServerApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Net.Sockets;

namespace ServerApp.Server
{
    internal partial class ServerLogic : IServerLogic
    {
        private const string CONFIG_PATH = @"..\..\..\Data\config.json";
        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true
        };

        private IPAddress _ip;
        private int _port;
        private IPEndPoint _endPoint;
        private Socket _listener;

        public ServerLogic()
        {
            ServerLoad();
        }

        private void ServerLoad()
        {
            if (!File.Exists(CONFIG_PATH))
            {
                File.Create(CONFIG_PATH).Close();
                Config defaultConfig = new("localhost", 12000);
                string jsonData = JsonSerializer.Serialize<Config>(defaultConfig, jsonSerializerOptions);
                File.WriteAllText(CONFIG_PATH, jsonData);
            }
            string data = File.ReadAllText(CONFIG_PATH);
            Config config = JsonSerializer.Deserialize<Config>(data, jsonSerializerOptions);
            if (!IPAddress.TryParse(config.IP, out _ip))
            {
                try
                {
                    _ip = Dns.GetHostEntry(config.IP).AddressList[0];
                }
                catch (Exception)
                {
                    Logger.Write($"Bad config IP: {config.IP}, set default \"localhost\"");
                    Config defaultConfig = new("localhost", 12000);
                    string jsonData = JsonSerializer.Serialize<Config>(defaultConfig, jsonSerializerOptions);
                    File.WriteAllText(CONFIG_PATH, jsonData);
                    _ip = Dns.GetHostEntry(defaultConfig.IP).AddressList[0];
                }
            }
            _port = config.Port;
            _endPoint = new(_ip, _port);
            _listener = new(_ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(_endPoint);
            _listener.Listen(10);
            Logger.Write($"The server is successfully loaded at: {_endPoint}");
        }
    }
}