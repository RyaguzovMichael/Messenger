using NetworkHelpers.Abstractions;
using System;
using System.Text.Json;

namespace NetworkHelpers.Packages;

public class LoginPackage : ISpecificPackage
{
    public LoginPackage(Guid iD, string login, string password, string name)
    {
        ID = iD;
        Login = login;
        Password = password;
        Name = name;
    }

    public Guid ID { get; private set; }
    public string Login { get; private set; }
    public string Password { get; private set; }
    public string Name { get; private set; }

    public Package ConvertToPackage() => new(PackageType.Login, JsonSerializer.Serialize(this, NetworkStreamExtensions.SerializerOptions));
}