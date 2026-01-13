using System.Net.Sockets;

namespace ClientApp
{
    internal class Program
    {
        static void ConnectServer(String server, int port)
        {
            string message, responseData;
            int bytes;
            try
            {
                // Create a TcpClient
                TcpClient client = new TcpClient(server, port);
                Console.Title = "Client Application";
                NetworkStream stream = null;

                while (true)
                {
                    Console.Write("Input message <press Enter to exit>:");
                    message = Console.ReadLine();
                    if (message == string.Empty)
                    {
                        break;
                    }

                    // Translate the passed message into ASCII and store it as a byte array.
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes($"{message}");

                    // Get a client stream for reading and writing.
                    stream = client.GetStream();

                    // Send the message to the connected TcpServer.
                    stream.Write(data, 0, data.Length);

                    Console.WriteLine("Sent: {0}", message);

                    // Receive the TcpServer response.
                    // Use buffer to store the response bytes.
                    data = new Byte[256];

                    // Read the first batch of the TcpServer response bytes.
                    bytes = stream.Read(data, 0, data.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    Console.WriteLine("Received: {0}", responseData);
                }
                // Shutdown and end connection
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }

        }//end ConnectServer

        static void Main(string[] args)
        {

            string server = "127.0.0.1";

            // Set the TcpListener on port 13000.
            int port = 13000;

            ConnectServer(server, port);

        }//end Main

    }//end Program
}

