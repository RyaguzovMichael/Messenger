using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace NetworkHelpers.Packages;

public class Package
{
    public PackageType Type { get; private set; }
    public string Data { get; private set; }

    public Package(PackageType type, string data)
    {
        Type = type;
        Data = data;
    }

    public static Package ConvertFromString(string jsonString) => JsonSerializer.Deserialize<Package>(jsonString, NetworkStreamExtensions.SerializerOptions);

    public override string ToString() => JsonSerializer.Serialize(this, NetworkStreamExtensions.SerializerOptions);


    public static explicit operator LoginPackage(Package package)
    {
        if (package.Type != PackageType.Login) throw new InvalidCastException("Package type is bad");
        return JsonSerializer.Deserialize<LoginPackage>(package.Data, NetworkStreamExtensions.SerializerOptions);
    }

    public static explicit operator LoginedUsersPackage(Package package)
    {
        if (package.Type != PackageType.LoginedUsers) throw new InvalidCastException("Package type is bad");
        return JsonSerializer.Deserialize<LoginedUsersPackage>(package.Data, NetworkStreamExtensions.SerializerOptions);
    }

    public static explicit operator MessagePackage(Package package)
    {
        if (package.Type != PackageType.Message) throw new InvalidCastException("Package type is bad");
        return JsonSerializer.Deserialize<MessagePackage>(package.Data, NetworkStreamExtensions.SerializerOptions);
    }

    public static explicit operator AddressedMessagePackage(Package package)
    {
        if (package.Type != PackageType.AddressedMessage) throw new InvalidCastException("Package type is bad");
        return JsonSerializer.Deserialize<AddressedMessagePackage>(package.Data, NetworkStreamExtensions.SerializerOptions);
    }
}