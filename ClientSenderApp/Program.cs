using EngineNetworkingLib;

using System.Net;
using System.Text;

namespace ClientSenderApp
{
    public enum Command
    {
        Invalid,
        Exit,
        Connect,
        Disconnect,
        SendData,
    }
    internal class Program
    {
        private static string DefaultIP = "127.0.0.1";
        private static int DefaultPort = 1336;
        private static ClientSender Client;
        private static bool isConnected;

        static void Main(string[] args)
        {
            Client = new ClientSender(IPAddress.Parse(DefaultIP), DefaultPort);
            bool isClosing = false;
            while (!isClosing)
            {
                Command validCommand = GetCommand();
                while (validCommand == Command.Invalid)
                {
                    Console.WriteLine("Invalid Command Entered, try Again");
                    validCommand = GetCommand();
                }

                switch (validCommand)
                {
                    case Command.SendData:
                        SendString();
                        break;
                    case Command.Connect:
                        isConnected = Connect();
                        break;
                    case Command.Disconnect:
                        Client.Disconnect();
                        isConnected = false;
                        break;
                    case Command.Exit:
                        isClosing = true;
                        break;
                    default:
                        throw new ArgumentException("Error occured trying to parse commands", nameof(validCommand));
                }
            }

            if (isConnected)
            {
                Client.Disconnect();
            }

            Client.Dispose();
        }
        public static Command GetCommand()
        {
            Console.WriteLine("Please Select a Command (Enter The Number) From the List Below:");
            Console.WriteLine("1: Exit");
            if (!isConnected)
            {
                Console.WriteLine("2: Connect");
            }
            else
            {
                Console.WriteLine("2: Send Data");
                Console.WriteLine("3: Disconnect");
            }


            Console.Write("Enter your choice here: ");
            var val = Console.ReadLine();

            if (!isConnected)
            {
                if (int.TryParse(val, out int choice))
                {
                    return choice switch
                    {
                        1 => Command.Exit,
                        2 => Command.Connect,
                        _ => Command.Invalid,
                    };
                }
                else
                {
                    return Command.Invalid;
                }
            }
            else
            {
                if (int.TryParse(val, out int choice))
                {
                    return choice switch
                    {
                        1 => Command.Exit,
                        2 => Command.Exit,
                        3 => Command.Disconnect,
                        _ => Command.Invalid,
                    };
                }
                else
                {
                    return Command.Invalid;
                }
            }
        }

        public static void SendString()
        {
            Console.Write("Please Enter string you would like to send: ");
            var str = Console.ReadLine();
            var returned = Encoding.ASCII.GetString(Client.SendReceiveData(Encoding.ASCII.GetBytes(str)).data);
            Console.WriteLine("Sent Data to Server");
            Console.WriteLine("Received Data from Server: Length {0}, message: {1}", returned.Length, returned);

        }

        private static bool Connect()
        {
            IPEndPoint? endpoint;
            while (!GetIpAndPort(out endpoint))
            {
                Console.WriteLine("Invalid IP and Port, Try Again");
            }
            Console.WriteLine("Connecting to Server...");
            bool ret = Client.Connect(endpoint!.Address, endpoint.Port);
            Console.WriteLine(ret ? "Connected to Server" : "Failed to Connect");
            return ret;
        }

        private static bool GetIpAndPort(out IPEndPoint? endPoint)
        {
            endPoint = null;
            Console.Write("Please Enter an IP: ");
            string I = Console.ReadLine().Trim();
            I = I == Environment.NewLine ? "" : I;
            Console.Write("Please Enter a Port: ");
            string P = Console.ReadLine().Trim();
            P = P == Environment.NewLine ? "" : P;

            if (string.IsNullOrEmpty(I) || string.IsNullOrWhiteSpace(I)) return false;
            if (string.IsNullOrEmpty(P) || string.IsNullOrWhiteSpace(P)) P = DefaultPort.ToString();
            return IPEndPoint.TryParse($"{I}:{P}", out endPoint);
        }
    }
}