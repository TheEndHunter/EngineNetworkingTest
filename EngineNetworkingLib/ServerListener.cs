using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EngineNetworkingLib
{
    public class ServerListener : IDisposable
    {
        private TcpListener mServer;
        private readonly int mBufferSize;
        public ServerListener(IPAddress address, int port, int maxConnections = 100, int bufferSize = 8192)
        {
            mServer = new TcpListener(address, port);
            mServer.Server.ReceiveBufferSize = bufferSize;
            mServer.Server.SendBufferSize = bufferSize;
            mBufferSize = bufferSize;
        }

        public void Run()
        {
            try
            {
                // Start listening for client requests.
                mServer.Start();

                // Buffer for reading data
                byte[] bytes = new byte[256];
                string data = string.Empty;

                // Enter the listening loop.
                while (true)
                {

                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    using TcpClient client = mServer.AcceptTcpClient();
                    var endpoint = client.Client.RemoteEndPoint;
                    Console.WriteLine("Connected!");

                    try
                    {
                        // Get a stream object for reading and writing
                        using NetworkStream stream = client.GetStream();

                        int i;

                        // Loop to receive all the data sent by the client.
                        while (client.Connected)
                        {
                            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                // Translate data bytes to a ASCII string.
                                data = Encoding.ASCII.GetString(bytes, 0, i);
                                Console.WriteLine("Received: {0}", data);

                                byte[] msg = Encoding.ASCII.GetBytes(data);
                                // Send back a response.
                                stream.Write(msg, 0, msg.Length);
                                Console.WriteLine("Sent: {0}", data);
                                stream.Flush();
                            }
                            client.Client.Shutdown(SocketShutdown.Both);
                        }
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine("Client[{0}] IO Error: {1}", endpoint, e.Message);
                    }
                    finally
                    {
                        // Shutdown and end connection
                        client.Close();
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                mServer.Stop();
            }
        }

        public void Dispose()
        {
            mServer?.Stop();
        }

        public ServerListener(string address, int port, int maxConnections = 100, int bufferSize = 256) : this(IPAddress.Parse(address), port, maxConnections, bufferSize)
        {
        }
    }
}