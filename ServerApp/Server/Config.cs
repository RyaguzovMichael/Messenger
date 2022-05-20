namespace ServerApp.Server
{
    internal class Config
    {  
        public string IP { get; private set; }
        public int Port { get; private set; }

        public Config(string iP, int port)
        {
            IP = iP;
            Port = port;
        }
    }
}