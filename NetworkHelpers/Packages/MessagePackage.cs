using NetworkHelpers.Abstractions;
using System;
using System.Text.Json;

namespace NetworkHelpers.Packages
{
    public class MessagePackage : ISpecificPackage
    {
        public MessagePackage(Guid autorID, string message, DateTime sendingTime)
        {
            AutorID = autorID;
            Message = message;
            SendingTime = sendingTime;
        }

        public Guid AutorID { get; private set; }
        public string Message { get; private set; }
        public DateTime SendingTime { get; private set; }
        public Package ConvertToPackage() => new(PackageType.Login, JsonSerializer.Serialize(this, NetworkStreamExtensions.SerializerOptions));
    }
}