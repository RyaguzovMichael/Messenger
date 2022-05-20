using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Text;
using System.Text.Json.Serialization;

namespace Packages
{
    public class Package
    {
        private static readonly JsonSerializerOptions serializerOptions = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
        };
        public const int MAX_PACKAGE_LENGTH = 2048;

        public DateTime DateTime { get; private set; }
        public PackageInfoType InfoType { get; private set; }
        public string Data { get; private set; }

        public Package(PackageInfoType infoType, string infoString)
        {
            DateTime = DateTime.Now;
            InfoType = infoType;
            Data = infoString;
        }

        [JsonConstructorAttribute]
        public Package(DateTime dateTime, PackageInfoType infoType, string data)
        {
            DateTime = dateTime;
            InfoType = infoType;
            Data = data;
        }

        public byte[] ConvertToByteArray()
        {
            byte[] result = Encoding.UTF8.GetBytes(this.ToString());
            if (result.Length <= MAX_PACKAGE_LENGTH)
            {
                return result;
            }
            throw new InvalidOperationException("Too large InfoString");
        }

        public static bool TryParce(string jsonString, out Package package)
        {
            try
            {
                package = JsonSerializer.Deserialize<Package>(jsonString, serializerOptions);
                return true;
            }
            catch (Exception)
            {
                package = null;
                return false;
            }
        }

        public override string ToString() => JsonSerializer.Serialize(this, serializerOptions);
    }
}