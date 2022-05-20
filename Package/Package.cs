using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Text;

namespace Package
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

        public Package(DateTime dateTime, PackageInfoType infoType, string infoString)
        {
            DateTime = dateTime;
            InfoType = infoType;
            Data = infoString;
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

        public static bool TryParce(byte[] data, out Package package)
        {
            try
            {
                string jsonString = Encoding.UTF8.GetString(data);
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