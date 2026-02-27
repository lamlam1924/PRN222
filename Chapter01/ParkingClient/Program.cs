// ===== IMPORT CÁC NAMESPACE CẦN THIẾT =====
using System.Net.Sockets;     // Làm việc với TCP Socket
using System.Text;            // Encoding (không dùng nhưng có import)

class Program
{
    // ===== HÀM MAIN - ĐIỂM BẮT ĐẦU CHƯƠNG TRÌNH =====
    static void Main()
    {
        // ===== NHẬP THÔNG TIN GATE =====
        // Mỗi client đại diện cho 1 cổng vào/ra (Gate1, Gate2, Gate3, Gate4)
        Console.Write("Enter Gate ID (Gate1 - Gate4): ");
        string gateId = Console.ReadLine();

        // ===== KẾT NỐI ĐẾN PARKING SERVER =====
        // 127.0.0.1 = localhost (server cùng máy)
        // 5000 = port của server
        TcpClient client = new TcpClient("127.0.0.1", 5000);
        
        // Lấy NetworkStream để giao tiếp
        NetworkStream stream = client.GetStream();

        // ===== TẠO READER VÀ WRITER ĐỂ GỬI/NHẬN TEXT =====
        // StreamReader = đọc dữ liệu text (từng dòng)
        StreamReader reader = new StreamReader(stream);
        
        // StreamWriter = ghi dữ liệu text
        // AutoFlush = true → tự động gửi ngay sau mỗi lần Write
        StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

        Console.WriteLine("Connected to server.");

        // ===== VÒNG LẶP GIAO TIẾP VỚI SERVER =====
        // Gate có thể thực hiện nhiều hành động (ENTER/EXIT) liên tục
        while (true)
        {
            // ===== NHẬP LỆNH TỪ NGƯỜI DÙNG =====
            // Người dùng (nhân viên gate) nhập lệnh
            Console.Write("ENTER | EXIT | QUIT: ");
            string command = Console.ReadLine().ToUpper();

            // ===== THOÁT CHƯƠNG TRÌNH =====
            // Nếu nhập QUIT → đóng kết nối và thoát
            if (command == "QUIT")
                break;

            // ===== TẠO VÀ GỬI TIN NHẮN ĐẾN SERVER =====
            // Format: "Gate1 ENTER" hoặc "Gate2 EXIT"
            string message = $"{gateId} {command}";
            
            // WriteLine() gửi 1 dòng text (kèm ký tự xuống dòng \n)
            // AutoFlush = true nên sẽ gửi ngay lập tức
            writer.WriteLine(message);

            // ===== NHẬN PHẢN HỒI TỪ SERVER =====
            // ReadLine() đọc 1 dòng phản hồi từ server
            // Các phản hồi có thể:
            //   - "OK 50" = thành công, hiện có 50 xe
            //   - "FULL" = bãi đã đầy
            //   - "INVALID" = lệnh không hợp lệ
            string response = reader.ReadLine();
            
            // Hiển thị phản hồi cho người dùng
            Console.WriteLine("Server: " + response);
        }

        // ===== ĐÓNG KẾT NỐI =====
        // Khi người dùng nhập QUIT, đóng kết nối với server
        client.Close();
    }
}
