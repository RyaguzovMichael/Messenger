using System;
using System.Collections.Generic;

namespace ServerApp.Abstractions;

internal interface IServer
{
    public int Port { get; set; }
    void StartServer();
    void RestartServer();
    void StopServer();
    Dictionary<Guid, string> GetLogginedUsers();
}