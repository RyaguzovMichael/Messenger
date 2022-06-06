using System.Net;

namespace ServerApp.Interfaces;

internal interface IServerLogic
{
    IPAddress IP { get; }
    public int Port { get; }
    public IPEndPoint EndPoint { get; }

    void StartServer();
    void RestartServer();
    bool StopServer();
    void SetIP(string ip);
    void SetPort(string port);
    void SetIPEndPoint(string ipEndPoint);
}