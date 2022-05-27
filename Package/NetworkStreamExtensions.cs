using NetworkHelpers.Abstractions;
using NetworkHelpers.Packages;
using System.Net.Sockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace NetworkHelpers;

public static class NetworkStreamExtensions
{
    public static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
    };

    public static void SendPackage(this NetworkStream stream, ISpecificPackage specificPackage)
    {
        Package package = specificPackage.ConvertToPackage();
        string jsonData = package.ToString();
        byte[] data = Encoding.Unicode.GetBytes(jsonData);
        stream.Write(data, 0, data.Length);
    }

    public static Package ReceivePackage(this NetworkStream stream)
    {
        byte[] data = new byte[64];
        StringBuilder builder = new();
        int bytesCount = 0;
        do
        {
            bytesCount = stream.Read(data, 0, data.Length);
            builder.Append(Encoding.Unicode.GetString(data, 0, bytesCount));
        } while (stream.DataAvailable);
        return Package.ConvertFromString(builder.ToString(0, builder.Length));
    }
}