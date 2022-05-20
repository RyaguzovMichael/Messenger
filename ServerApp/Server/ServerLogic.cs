using ServerApp.Interfaces;
using ServerApp.Helpers;
using Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace ServerApp.Server;

internal partial class ServerLogic : IServerLogic
{
    private const string CONFIG_PATH = @".\Data";
    private const string CONFIG_FILE_NAME = @"config.json";
    private readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    private IPAddress _ip;
    private int _port;
    private IPEndPoint _endPoint;
    private Socket _listener;
    private bool _serverIsVorking = true;
    private Thread _serverMainTread;

    public ServerLogic()
    {
        ServerLoad();
    }

    private void ServerLoad()
    {
        if (!Directory.Exists(CONFIG_PATH)) Directory.CreateDirectory(CONFIG_PATH);
        if (!File.Exists(Path.Combine(CONFIG_PATH,CONFIG_FILE_NAME)))
        {
            File.Create(Path.Combine(CONFIG_PATH, CONFIG_FILE_NAME)).Close();
            Config defaultConfig = new("localhost", 12000);
            string jsonData = JsonSerializer.Serialize<Config>(defaultConfig, jsonSerializerOptions);
            File.WriteAllText(Path.Combine(CONFIG_PATH, CONFIG_FILE_NAME), jsonData);
        }
        string data = File.ReadAllText(Path.Combine(CONFIG_PATH, CONFIG_FILE_NAME));
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
                File.WriteAllText(Path.Combine(CONFIG_PATH, CONFIG_FILE_NAME), jsonData);
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

    public void StartServer()
    {
        _serverMainTread = new(StartListening);
        _serverMainTread.Start();
        Logger.Write("The main thread is up and running");
    }

    private void StartListening()
    {

        try
        {
            while (_serverIsVorking)
            {
                Socket handler = _listener.Accept();
                Logger.Write($"A user with the address {handler.RemoteEndPoint} connected to server");
                byte[] bytes = new byte[Package.MAX_PACKAGE_LENGTH];
                int bytesCount = handler.Receive(bytes);
                string data = Encoding.UTF8.GetString(bytes, 0, bytesCount);
                Logger.Write($"Received package Data: {data}");

                Package reply;
                if (!Package.TryParce(data, out Package package))
                {
                    Logger.Write($"A bad data packet was received.");
                    reply = new Package(PackageInfoType.BadPackage, "");
                }
                else
                {
                    Logger.Write($"Received data packet whith type \"{package.InfoType}\"");
                    reply = GenerateReply(package);
                }

                byte[] replyBytes = reply.ConvertToByteArray();
                handler.Send(replyBytes);
                Logger.Write($"Sended data packet whith type \"{reply.InfoType}\"");
                Logger.Write($"A user with the address {handler.RemoteEndPoint} disconnected.");
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }
        catch (Exception ex)
        {
            Logger.Write(ex.Message);
        }
    }

    private Package GenerateReply(Package package)
    {
        throw new NotImplementedException();
    }
}