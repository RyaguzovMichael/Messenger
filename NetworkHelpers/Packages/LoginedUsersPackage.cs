using NetworkHelpers.Abstractions;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace NetworkHelpers.Packages;

public class LoginedUsersPackage : ISpecificPackage
{
    public LoginedUsersPackage(Dictionary<Guid, string> loginedUsers, bool isConnected)
    {
        LoginedUsers = loginedUsers;
        IsConnected = isConnected;
    }

    public Dictionary<Guid, string> LoginedUsers { get; private set; }
    public bool IsConnected { get; private set; }


    public Package ConvertToPackage() => new(PackageType.Login, JsonSerializer.Serialize(this, NetworkStreamExtensions.SerializerOptions));
}