using NetworkHelpers.Abstractions;
using ServerApp.Abstractions;
using ServerApp.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerApp.Server;

internal class ServerLogic : IServer
{
    private int _port;
    private TcpListener _listener;
    private List<Connection> _connections;
    private bool _serverIsVorking;

    public int Port
    {
        get => _port;
        set
        {
            if (value >= 49152 && value <= 65535)
            {
                _port = value;
                Logger.Write($"Port set on: {_port}");
                ConfigInteractor.SaveConfig(new Config("localhost", Port));
            }
            else
            {
                throw new ArgumentOutOfRangeException("Port", "Port can stand only in range 49152-65535");
            }
        }
    }

    public ServerLogic()
    {
        Config config = ConfigInteractor.LoadConfig();
        _port = config.Port;
        _connections = new();
        _serverIsVorking = false;
        _listener = new(IPAddress.Any, _port);
    }

    public void StartServer()
    {
        if (_serverIsVorking) return;
        _serverIsVorking = true;
        Thread listen = new(Listen);
        listen.Start();
        Logger.Write("The main thread is up and running");
    }

    public void StopServer()
    {
        if (!_serverIsVorking) return;
        _serverIsVorking = false;
        _listener.Stop();
        foreach (Connection connection in _connections) connection.Close();
        Logger.Write("Server is stopped");
    }

    public void RestartServer()
    {
        StopServer();
        StartServer();
        Logger.Write("Server is restarted");
    }

    public Dictionary<Guid, string> GetLogginedUsers()
    {
        Dictionary<Guid, string> users = new();
        foreach (Connection connection in _connections)
        {
            if (connection.ID != Guid.Empty) users.Add(connection.ID, connection.UserName);
        }
        return users;
    }

    internal void BroadcastMessage(ISpecificPackage package, Connection ignoredConnection)
    {
        foreach (Connection connection in _connections)
        {
            if (connection != ignoredConnection) connection.SendPackage(package);
        }
    }

    internal void RemoveConnection(Connection connection)
    {
        _connections.Remove(connection);
    }

    private void Listen()
    {
        try
        {
            _listener.Start();
            Logger.Write($"The server is successfully loaded, and listen port: {_port}");
            while (_serverIsVorking)
            {
                try
                {
                    TcpClient tcpClient = _listener.AcceptTcpClient();
                    Connection connection = new(tcpClient, this);
                    Thread clientThread = new(connection.Process);
                    _connections.Add(connection);
                    clientThread.Start();
                }
                catch (Exception ex)
                {
                    Logger.Write(ex.Message);
                    StopServer();
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Write(ex.Message);
            StopServer();
        }
    }
}