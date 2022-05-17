using System.Net;
using System.Net.Sockets;

namespace EngineNetworkingLib
{
    public class ClientSender : IDisposable
    {
        private TcpClient mClient;
        private NetworkStream mNetworkStream;
        private readonly int mBufferSize;
        private readonly IPEndPoint mDefaultEndpoint;
        private readonly LingerOption mDefaultLinger;
        public ClientSender(IPAddress address, int port, int bufferSize = 8192)
        {
            mDefaultEndpoint = new IPEndPoint(address, port);
            mDefaultLinger = new LingerOption(true, 0);
            mBufferSize = bufferSize;
        }

        public bool Connect(IPAddress address, int port)
        {
            try
            {
                if (mClient != null && mClient.Connected)
                {
                    Disconnect();
                }
                mClient = new TcpClient()
                {
                    ReceiveBufferSize = mBufferSize,
                    SendBufferSize = mBufferSize,
                    LingerState = mDefaultLinger,
                    ExclusiveAddressUse = false,
                };
                mClient.Client.Bind(mDefaultEndpoint);
                mClient.Connect(address, port);
                mNetworkStream = mClient.GetStream();
                Console.WriteLine("Connnected to server at endpoint: {0}:{1}", address, port);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not connect to server at endpoint: {0}:{1}, with exception {2}", address, port, e.Message);
                return false;
            }
        }

        public (int length, byte[] data) SendReceiveData(ReadOnlySpan<byte> data)
        {
            mNetworkStream.Write(data);
            mNetworkStream.Flush();
            byte[] received = new byte[mBufferSize];
            return (mNetworkStream.Read(received), received);
        }

        public void Disconnect()
        {
            if (mClient.Connected)
            {
                mNetworkStream.Flush();
                mNetworkStream.Close();
                mNetworkStream.Dispose();
                mClient.Close();
                mClient.Dispose();
            }
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}