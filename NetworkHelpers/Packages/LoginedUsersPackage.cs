using NetworkHelpers.Abstractions;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace NetworkHelpers.Packages;

public class LoginedUsersPackage : ISpecificPackage
{
    public Dictionary<Guid, string> LoginedUsers { get; private set; }

    public LoginedUsersPackage(Dictionary<Guid, string> loginedUsers)
    {
        LoginedUsers = loginedUsers;
    }

    public Package ConvertToPackage() => new(PackageType.Login, JsonSerializer.Serialize(this, NetworkStreamExtensions.SerializerOptions));
}