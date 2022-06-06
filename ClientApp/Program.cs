using NetworkHelpers;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

try
{
    while (true)
    {
        Console.Write("Введите сообщение: ");
        string message = Console.ReadLine();
        SendMessage("192.168.1.5", 50000, message);
        Console.WriteLine();
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    Console.ReadLine();
}

static void SendMessage(string ipAddress, int port, string message)
{
    IPAddress ip = IPAddress.Parse(ipAddress);
    IPEndPoint endPoint = new(ip, port);

    Socket sender = new(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    sender.Connect(endPoint);

    Console.WriteLine($"Сокет соединяется с {sender.RemoteEndPoint}");
    Package send = new(PackageInfoType.Ping, "");
    byte[] sendingBytes = send.ConvertToByteArray();
    sender.Send(sendingBytes);

    byte[] receivedBytes = new byte[Package.MAX_PACKAGE_LENGTH];
    int bytesCount = sender.Receive(receivedBytes);
    string data = Encoding.UTF8.GetString(receivedBytes, 0, bytesCount);
    if (!Package.TryParce(data, out Package package))
    {
        Console.WriteLine($"A bad data packet was received.");
    }
    else
    {
        Console.WriteLine($"Received data packet whith type {package.InfoType} and Data: {package.Data}");
    }

    sender.Shutdown(SocketShutdown.Both);
    sender.Close();
}