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

public delegate Package GenerateRequest();

internal class ServerLogic : IServerLogic
{
    private IPAddress _ip;
    private int _port;
    private IPEndPoint _endPoint;
    private Socket _listener;
    private bool _serverIsVorking = true;
    private Thread _serverMainTread;
    private readonly Dictionary<PackageInfoType, GenerateRequest> _requestGeneratorsDictionary;

    public IPAddress IP 
    { 
        get => _ip; 
        private set
        {
            _ip = value;
            Logger.Write($"IP set on: {_ip}");
            ConfigInteractor.SaveConfig(new Config(IP.ToString(), Port));
        }
    }

    public int Port 
    { 
        get => _port;
        private set
        {
            if (value >= 49152 && value <= 65535)
            {
                _port = value;
                Logger.Write($"Port set on: {_port}");
                ConfigInteractor.SaveConfig(new Config(IP.ToString(), Port));
            }
            else
            {
                throw new ArgumentOutOfRangeException("Port can stand only in range 49152-65535");
            }
        }
    }
    public IPEndPoint EndPoint 
    { 
        get => _endPoint;
        private set
        {
            _ip = value.Address;
            _port = value.Port;
            _endPoint = value;
            Logger.Write($"Endpoint set on: {_endPoint}");
            ConfigInteractor.SaveConfig(new Config(IP.ToString(), Port));
        }
    }

    public ServerLogic()
    {
        ServerLoad();
        _requestGeneratorsDictionary = new()
        {
            { PackageInfoType.Ping, GeneratePingRequest }
        };
    }

    public void StartServer()
    {
        if(_serverMainTread == null || _serverMainTread.ThreadState != ThreadState.Running)
        {
            _serverMainTread = new(StartListening);
            _serverMainTread.Start();
            Logger.Write("The main thread is up and running");
        }
        else
        {
            throw new InvalidOperationException("The server is already running");
        }
    }

    public void RestartServer()
    {
        if (StopServer())
        {
            _serverIsVorking = true;
            StartServer();
            Logger.Write("Server is restarted");
        }
    }

    public bool StopServer()
    {
        if (_serverMainTread.ThreadState == ThreadState.Running)
        {
            _serverIsVorking = false;
            try
            {
                //TODO: Переделать остановку сервера, устаревший метод Abort.
                _serverMainTread.Abort();
            }
            catch (Exception ex)
            {
                Logger.Write($"Exception on server: {ex.Message}");
                throw new InvalidOperationException($"Exception on server: {ex.Message}");
            }
            Logger.Write("Server is stopped");
        }
        return true;
    }

    public void SetIP(string ip) => IP = IPAddress.Parse(ip);

    public void SetPort(string port) => Port = int.Parse(port);

    public void SetIPEndPoint(string ipEndPoint) => EndPoint = IPEndPoint.Parse(ipEndPoint);

    private void ServerLoad()
    {
        Config config = ConfigInteractor.LoadConfig();
        _ip = IPAddress.Parse(config.IP);
        _port = config.Port;
        _endPoint = new(IP, Port);
        _listener = new(IP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _listener.Bind(EndPoint);
        _listener.Listen(10);
        Logger.Write($"The server is successfully loaded at: {EndPoint}");
    }

    private void StartListening()
    {
        while (_serverIsVorking)
        {
            try
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
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
        }
    }

    private Package GenerateReply(Package package)
    {
        if(!_requestGeneratorsDictionary.TryGetValue(package.InfoType, out GenerateRequest replyGenerator))
        {
            Logger.Write($"An unsupported request was received \"{package.InfoType}\"");
            return new Package(PackageInfoType.NotSupportedRequest, $"The \"{package.InfoType}\" request not supported now");
        }
        return replyGenerator.Invoke();
    }

    private Package GeneratePingRequest()
    {
        Logger.Write("Ping request generated");
        return new Package(PackageInfoType.Ping, "");
    }
}