using EngineNetworkingLib;

namespace ServerListenerApp
{
    internal class Program
    {
        private static readonly string IP = "127.0.0.1";
        private static readonly int DefaultPort = 1337;
        static void Main(string[] args)
        {
            using ServerListener listener = new(IP, DefaultPort);
            listener.Run();
        }
    }
}