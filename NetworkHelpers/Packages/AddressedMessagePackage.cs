using NetworkHelpers.Abstractions;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace NetworkHelpers.Packages
{
    public class AddressedMessagePackage : ISpecificPackage
    {
        public AddressedMessagePackage(List<Guid> addreseees, Guid autorID, DateTime time, string message)
        {
            Addreseees = addreseees;
            AutorID = autorID;
            Time = time;
            Message = message;
        }

        public List<Guid> Addreseees { get; private set; }
        public Guid AutorID { get; private set; }
        public DateTime Time { get; private set; }
        public string Message { get; private set; }
        public Package ConvertToPackage() => new(PackageType.Login, JsonSerializer.Serialize(this, NetworkStreamExtensions.SerializerOptions));
    }
}