// ===== IMPORT CÁC NAMESPACE CẦN THIẾT =====
using System.Net;             // Làm việc với địa chỉ IP
using System.Net.Sockets;     // Làm việc với TCP Socket
using System.Text;            // Encoding (không dùng trong code này nhưng có import)

class Program
{
    // ===== BIẾN TOÀN CỤC - TRẠNG THÁI BÃI ĐỖ XE =====
    
    // Sức chứa tối đa của bãi đỗ xe (100 xe)
    static int capacity = 2;
    
    // Số xe hiện tại đang đỗ trong bãi
    static int currentCars = 0;
    
    // ===== OBJECT LOCKER CHO THREAD SAFETY =====
    // readonly object = object dùng để lock (đồng bộ hóa giữa các thread)
    // 
    // Tại sao cần lock?
    //   - Server phục vụ NHIỀU gate (client) cùng lúc
    //   - Nhiều thread cùng truy cập biến currentCars
    //   - Không lock → race condition → dữ liệu sai!
    // 
    // Ví dụ race condition:
    //   Thread 1: đọc currentCars = 50
    //   Thread 2: đọc currentCars = 50
    //   Thread 1: currentCars++ → 51
    //   Thread 2: currentCars++ → 51 (SHOULD BE 52!)
    static readonly object locker = new object();

    // ===== HÀM MAIN - KHỞI ĐỘNG SERVER =====
    static void Main()
    {
        // ===== TẠO TCP LISTENER =====
        // IPAddress.Any = lắng nghe trên TẤT CẢ network interface
        //   - 127.0.0.1 (localhost)
        //   - 192.168.x.x (LAN)
        //   - v.v.
        // Port 5000 = cổng server lắng nghe
        TcpListener server = new TcpListener(IPAddress.Any, 5000);
        server.Start();

        Console.WriteLine("=== Parking Server Started (Port 5000) ===");

        // ===== VÒNG LẶP VÔ HẠN - CHẤP NHẬN CLIENT =====
        // Server luôn chạy và chờ các gate kết nối
        while (true)
        {
            // ===== CHẤP NHẬN KẾT NỐI TỪ GATE (CLIENT) =====
            // AcceptTcpClient() chờ cho đến khi có gate kết nối
            TcpClient client = server.AcceptTcpClient();
            
            // ===== TẠO THREAD MỚI ĐỂ XỬ LÝ GATE =====
            // Mỗi gate có 1 thread riêng
            // → Nhiều gate có thể hoạt động đồng thời
            Thread t = new Thread(HandleClient);
            t.Start(client);
        }
    }

    // ===== HÀM XỬ LÝ MỘT GATE (CLIENT) =====
    // Chạy trên thread riêng cho mỗi gate
    static void HandleClient(object obj)
    {
        // Ép kiểu object → TcpClient
        TcpClient client = (TcpClient)obj;

        // ===== TẠO STREAM ĐỂ GIAO TIẾP =====
        // using = tự động dispose/close khi ra khỏi scope
        using NetworkStream stream = client.GetStream();
        
        // StreamReader = đọc dữ liệu dạng text (từng dòng)
        using StreamReader reader = new StreamReader(stream);
        
        // StreamWriter = ghi dữ liệu dạng text
        // AutoFlush = true → tự động flush sau mỗi lần Write (gửi ngay)
        using StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

        // ===== VÒNG LẶP NHẬN VÀ XỬ LÝ YÊU CẦU =====
        // Gate có thể gửi nhiều request (ENTER/EXIT) liên tục
        while (true)
        {
            // ===== ĐỌC YÊU CẦU TỪ GATE =====
            // ReadLine() đọc 1 dòng text
            // null = gate đã ngắt kết nối → thoát vòng lặp
            string? request = reader.ReadLine();
            if (request == null) break;

            // ===== XỬ LÝ YÊU CẦU =====
            // Gọi hàm ProcessRequest để xử lý logic nghiệp vụ
            string response = ProcessRequest(request);
            
            // ===== GỬI PHẢN HỒI VE GATE =====
            writer.WriteLine(response);
        }

        // ===== ĐÓNG KẾT NỐI =====
        client.Close();
    }

    // ===== HÀM XỬ LÝ YÊU CẦU - LOGIC NGHIỆP VỤ =====
    static string ProcessRequest(string request)
    {
        // ===== PHÂN TÍCH YÊU CẦU =====
        // Format: "Gate1 ENTER" hoặc "Gate2 EXIT"
        // Split() tách chuỗi thành mảng
        // StringSplitOptions.RemoveEmptyEntries = bỏ qua khoảng trắng thừa
        string[] parts = request.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        // Kiểm tra format đúng không (phải có 2 phần)
        if (parts.Length != 2) return "INVALID";

        string gate = parts[0];        // Tên gate (Gate1, Gate2...)
        string command = parts[1].ToUpper();  // Lệnh (ENTER hoặc EXIT)

        // ===== ĐỒN BỘ HÓA VỚI LOCK =====
        // lock(locker) đảm bảo chỉ 1 thread được vào block này tại 1 thời điểm
        // → Tránh race condition khi nhiều gate cùng ENTER/EXIT
        lock (locker)
        {
            // ===== XỬ LÝ LỆNH ENTER =====
            if (command == "ENTER")
            {
                // Kiểm tra bãi đã đầy chưa
                if (currentCars >= capacity)
                    return "FULL";  // Trả về "FULL" nếu hết chỗ

                // Tăng số xe đang đỗ
                currentCars++;
                
                // Log ra console (server-side)
                Console.WriteLine($"[{gate}] ENTER -> {currentCars}");
                
                // Trả về "OK" + số xe hiện tại
                return $"OK {currentCars}";
            }

            // ===== XỬ LÝ LỆNH EXIT =====
            if (command == "EXIT")
            {
                // Giảm số xe (nếu có xe trong bãi)
                if (currentCars > 0)
                    currentCars--;

                // Log ra console
                Console.WriteLine($"[{gate}] EXIT -> {currentCars}");
                
                // Trả về "OK" + số xe hiện tại
                return $"OK {currentCars}";
            }
        }

        // Lệnh không hợp lệ (không phải ENTER hay EXIT)
        return "INVALID";
    }
}
