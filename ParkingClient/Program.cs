using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main()
    {
        Console.Write("Enter Gate ID (Gate1 - Gate4): ");
        string gateId = Console.ReadLine();

        TcpClient client = new TcpClient("127.0.0.1", 5000);
        NetworkStream stream = client.GetStream();

        StreamReader reader = new StreamReader(stream);
        StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

        Console.WriteLine("Connected to server.");

        while (true)
        {
            Console.Write("ENTER | EXIT | QUIT: ");
            string command = Console.ReadLine().ToUpper();

            if (command == "QUIT")
                break;

            string message = $"{gateId} {command}";
            writer.WriteLine(message); // gửi 1 dòng

            string response = reader.ReadLine();
            Console.WriteLine("Server: " + response);
        }

        client.Close();
    }
}