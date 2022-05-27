using NetworkHelpers;
using NetworkHelpers.Abstractions;
using NetworkHelpers.Packages;
using ServerApp.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text.Json;

namespace ServerApp.Server
{
    internal class Connection
    {
        internal Guid ID { get; private set; }
        internal string UserName { get; private set; }

        private readonly NetworkStream _stream;
        private readonly TcpClient _tcpClient;
        private readonly ServerLogic _server;
        private delegate void GenerateRequest(Package package);
        private readonly Dictionary<PackageType, GenerateRequest> _requestGeneratorsDictionary;

        internal Connection(TcpClient tcpClient, ServerLogic server)
        {
            _tcpClient = tcpClient;
            _stream = _tcpClient.GetStream();
            _server = server;
            ID = Guid.Empty;
            UserName = "";
            _requestGeneratorsDictionary = new()
            {
                {PackageType.Login, AcceptLoginPackage},
                {PackageType.LoginedUsers, AcceptLoginedUsersPackage },
                {PackageType.Message, AcceptMessagePackage },
                {PackageType.AddressedMessage, AcceptAddressedMessagePackage }
            };
        }

        internal void Process()
        {
            while (true)
            {
                try
                {
                    Package package = _stream.ReceivePackage();
                    _requestGeneratorsDictionary[package.Type].Invoke(package);
                }
                catch (KeyNotFoundException)
                {
                    Logger.Write($"An unsupported package arrived.");
                    continue;
                }
                catch (JsonException)
                {
                    Logger.Write($"Bad package arrived.");
                    continue;
                }
                catch (ObjectDisposedException)
                {
                    Logger.Write($"{UserName} left the chat room");
                    Close();
                    break;
                }
                catch (Exception ex)
                {
                    Logger.Write(ex.Message);
                    Close();
                    break;
                }
            }
        }

        internal void SendPackage(ISpecificPackage package)
        {
            if (package is AddressedMessagePackage addressedMessagePackage)
            {
                foreach (Guid id in addressedMessagePackage.Addresees)
                {
                    if (id == ID) _stream.SendPackage(addressedMessagePackage);
                }
            }
            else
            {
                _stream.SendPackage(package);
            }
        }

        internal void Close()
        {
            _server.BroadcastMessage(new LoginedUsersPackage(new Dictionary<Guid, string>() { { ID, UserName } }, false), this);
            _server.RemoveConnection(this);
            _stream?.Close();
            _tcpClient?.Close();
        }

        private void AcceptLoginPackage(Package package)
        {
            LoginPackage loginPackage = (LoginPackage)package;
            if (ID == Guid.Empty)
            {
                ID = ServerLogin.TryLogin(loginPackage.Login, loginPackage.Password);
                UserName = loginPackage.Name;
            }
            SendPackage(new LoginPackage(ID, "", "", UserName));
            if (ID != Guid.Empty) _server.BroadcastMessage(new LoginedUsersPackage(new Dictionary<Guid, string>() { { ID, UserName } }, true), this);
        }

        private void AcceptMessagePackage(Package package)
        {
            _server.BroadcastMessage((MessagePackage)package, this);
        }

        private void AcceptAddressedMessagePackage(Package package)
        {
            _server.BroadcastMessage((AddressedMessagePackage)package, this);
        }

        private void AcceptLoginedUsersPackage(Package package)
        {
            SendPackage(new LoginedUsersPackage(_server.GetLogginedUsers(), true));
        }
    }
}