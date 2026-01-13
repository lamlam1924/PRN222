using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static int capacity = 100;
    static int currentCars = 0;
    static readonly object locker = new object();

    static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, 5000);
        server.Start();

        Console.WriteLine("=== Parking Server Started (Port 5000) ===");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Thread t = new Thread(HandleClient);
            t.Start(client);
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;

        using NetworkStream stream = client.GetStream();
        using StreamReader reader = new StreamReader(stream);
        using StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

        while (true)
        {
            string? request = reader.ReadLine();
            if (request == null) break;

            string response = ProcessRequest(request);
            writer.WriteLine(response);
        }

        client.Close();
    }

    static string ProcessRequest(string request)
    {
        // Format: Gate1 ENTER
        string[] parts = request.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2) return "INVALID";

        string gate = parts[0];
        string command = parts[1].ToUpper();

        lock (locker)
        {
            if (command == "ENTER")
            {
                if (currentCars >= capacity)
                    return "FULL";

                currentCars++;
                Console.WriteLine($"[{gate}] ENTER -> {currentCars}");
                return $"OK {currentCars}";
            }

            if (command == "EXIT")
            {
                if (currentCars > 0)
                    currentCars--;

                Console.WriteLine($"[{gate}] EXIT -> {currentCars}");
                return $"OK {currentCars}";
            }
        }

        return "INVALID";
    }
}
