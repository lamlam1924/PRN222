// ===== IMPORT CÁC NAMESPACE CẦN THIẾT =====
using System.IO;              // Làm việc với file và stream
using System.Net;             // Làm việc với địa chỉ IP
using System.Net.Sockets;     // Làm việc với Socket và TCP/IP

namespace ServerApp
{
    class Program
    {
        // ===== HÀM XỬ LÝ TIN NHẮN TỪ CLIENT (CHẠY TRÊN THREAD RIÊNG) =====
        // Hàm này xử lý giao tiếp với MỘT client cụ thể
        // Mỗi client sẽ có 1 thread riêng để xử lý
        static void ProcessMessage(object parm)
        {
            string data;
            int count;
            try
            {
                // Ép kiểu (cast) object về TcpClient
                // TcpClient = đại diện cho kết nối với 1 client
                TcpClient client = parm as TcpClient;

                // ===== TẠO BUFFER ĐỂ NHẬN DỮ LIỆU =====
                // Buffer = vùng nhớ tạm để lưu dữ liệu nhận được
                // 256 bytes = có thể nhận tối đa 256 ký tự mỗi lần
                Byte[] bytes = new Byte[256];

                // ===== LẤY STREAM ĐỂ GIAO TIẾP =====
                // NetworkStream = "ống dẫn" để gửi/nhận dữ liệu qua mạng
                NetworkStream stream = client.GetStream();

                // ===== VÒNG LẶP NHẬN VÀ XỬ LÝ DỮ LIỆU =====
                // Lặp liên tục để nhận tin nhắn từ client
                // stream.Read() đọc dữ liệu và trả về số bytes đã đọc
                // Nếu count = 0 → client đã ngắt kết nối → thoát vòng lặp
                while ((count = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // ===== CHUYỂN ĐỔI DỮ LIỆU TỪ BYTES → STRING =====
                    // Encoding.ASCII.GetString() chuyển mảng bytes thành chuỗi văn bản
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, count);
                    
                    // In ra tin nhắn nhận được + thời gian
                    // {DateTime.Now:t} = định dạng thời gian ngắn (VD: 14:30)
                    Console.WriteLine($"Received: {data} at {DateTime.Now:t}");

                    // ===== XỬ LÝ DỮ LIỆU =====
                    // Server đơn giản này chỉ chuyển text thành CHỮ HOA
                    // (Trong thực tế, đây có thể là logic phức tạp hơn)
                    data = $"{data.ToUpper()}";
                    
                    // Chuyển chuỗi đã xử lý thành mảng bytes để gửi lại
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                    // ===== GỬI PHẢN HỒI VỀ CLIENT =====
                    // Write() gửi dữ liệu qua stream
                    stream.Write(msg, 0, msg.Length);
                    Console.WriteLine($"Sent: {data}");
                }
                
                // ===== ĐÓNG KẾT NỐI VỚI CLIENT =====
                // Khi client ngắt kết nối (count = 0), đóng TcpClient
                client.Close();
            }
            catch (Exception ex)
            {
                // Bắt lỗi nếu có vấn đề trong quá trình giao tiếp
                Console.WriteLine("{0}", ex.Message);
                Console.WriteLine("Waiting message...");
            }

        }//end ProcessMessage

        // ===== HÀM KHỞI ĐỘNG VÀ CHẠY SERVER =====
        static void ExecuteServer(string host, int port)
        {
            int Count = 0;  // Đếm số lượng client đã kết nối
            TcpListener server = null;
            try
            {
                // Đặt tiêu đề cửa sổ console
                Console.Title = "Server Application";
                
                // ===== TẠO ĐỊA CHỈ IP =====
                // Parse() chuyển chuỗi "127.0.0.1" thành object IPAddress
                // 127.0.0.1 = localhost (máy tính của bạn)
                IPAddress localAddr = IPAddress.Parse(host);
                
                // ===== TẠO TCP LISTENER =====
                // TcpListener = server lắng nghe kết nối từ client
                // Tham số: địa chỉ IP và port (cổng) để lắng nghe
                server = new TcpListener(localAddr, port);

                // ===== BẮT ĐẦU LẮNG NGHE =====
                // Start() bắt đầu lắng nghe kết nối từ client
                server.Start();
                Console.WriteLine(new string('*', 40));
                Console.WriteLine("Waiting for a connection... ");

                // ===== VÒNG LẶP VÔ HẠN - CHẤP NHẬN CLIENT =====
                // Server luôn chạy và chờ client kết nối
                while (true)
                {
                    // ===== CHẤP NHẬN KẾT NỐI TỪ CLIENT =====
                    // AcceptTcpClient() chờ (block) cho đến khi có client kết nối
                    // Khi có client kết nối → trả về TcpClient object
                    TcpClient client = server.AcceptTcpClient();
                    
                    // Tăng biến đếm và hiển thị số client đã kết nối
                    Console.WriteLine($"Number of client connected: {++Count}");
                    Console.WriteLine(new string('*', 40));

                    // ===== TẠO THREAD MỚI ĐỂ XỬ LÝ CLIENT =====
                    // Tại sao cần Thread?
                    //   - Server cần phục vụ NHIỀU client cùng lúc
                    //   - Mỗi client có 1 thread riêng → không block nhau
                    //   - Server có thể tiếp tục AcceptTcpClient() để nhận client mới
                    Thread thread = new Thread(new ParameterizedThreadStart(ProcessMessage));
                    
                    // Start() chạy thread và truyền client object vào hàm ProcessMessage
                    thread.Start(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
            }
            finally
            {
                // ===== DỪNG SERVER VÀ DỌN DẸP =====
                // Khi có exception hoặc thoát vòng lặp → dừng server
                server.Stop();
                Console.WriteLine("Server stopped. Press any key to exit !");
            }

            Console.Read();
        }

        // ===== HÀM MAIN - ĐIỂM BẮT ĐẦU CHƯƠNG TRÌNH =====
        public static void Main()
        {
            // ===== CẤU HÌNH SERVER =====
            // Host: 127.0.0.1 = localhost (chỉ chấp nhận kết nối từ máy local)
            string host = "127.0.0.1";

            // Port: 13000 = cổng mà server lắng nghe
            // Client phải kết nối đúng port này
            int port = 13000;

            // Khởi động server
            ExecuteServer(host, port);
        }//end Main
    }
}
