// ===== IMPORT NAMESPACE CẦN THIẾT =====
using System.Net.Sockets;     // Làm việc với Socket và TCP/IP

namespace ClientApp
{
    internal class Program
    {
        // ===== HÀM KẾT NỐI VÀ GIAO TIẾP VỚI SERVER =====
        static void ConnectServer(String server, int port)
        {
            string message, responseData;
            int bytes;
            try
            {
                // ===== TẠO KẾT NỐI ĐẾN SERVER =====
                // TcpClient kết nối đến server với địa chỉ IP và port
                // Nếu server không chạy hoặc từ chối kết nối → throw exception
                TcpClient client = new TcpClient(server, port);
                
                // Đặt tiêu đề cửa sổ console
                Console.Title = "Client Application";
                
                // Khai báo NetworkStream (sẽ khởi tạo trong vòng lặp)
                NetworkStream stream = null;

                // ===== VÒNG LẶP GỬI VÀ NHẬN TIN NHẮN =====
                // Vòng lặp vô hạn để người dùng có thể gửi nhiều tin nhắn
                while (true)
                {
                    // ===== NHẬP TIN NHẮN TỪ NGƯỜI DÙNG =====
                    Console.Write("Input message <press Enter to exit>:");
                    message = Console.ReadLine();
                    
                    // Nếu người dùng nhấn Enter (chuỗi rỗng) → thoát chương trình
                    if (message == string.Empty)
                    {
                        break;
                    }

                    // ===== CHUYỂN ĐỔI TIN NHẮN THÀNH BYTES =====
                    // Mạng chỉ truyền dữ liệu dạng bytes, không phải string
                    // Encoding.ASCII.GetBytes() chuyển string → byte array
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes($"{message}");

                    // ===== LẤY STREAM ĐỂ GIAO TIẾP =====
                    // NetworkStream = "ống dẫn" để gửi/nhận dữ liệu qua mạng
                    stream = client.GetStream();

                    // ===== GỬI TIN NHẮN ĐẾN SERVER =====
                    // Write() gửi mảng bytes qua network stream
                    // Tham số:
                    //   - data: mảng bytes cần gửi
                    //   - 0: bắt đầu từ vị trí 0
                    //   - data.Length: gửi toàn bộ mảng
                    stream.Write(data, 0, data.Length);

                    // Thông báo đã gửi tin nhắn
                    Console.WriteLine("Sent: {0}", message);

                    // ===== NHẬN PHẢN HỒI TỪ SERVER =====
                    
                    // Tạo buffer để lưu dữ liệu nhận được (tối đa 256 bytes)
                    data = new Byte[256];

                    // Đọc dữ liệu từ stream
                    // Read() trả về số bytes đã đọc được
                    bytes = stream.Read(data, 0, data.Length);
                    
                    // Chuyển đổi bytes nhận được thành string
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    
                    // Hiển thị phản hồi từ server
                    Console.WriteLine("Received: {0}", responseData);
                }
                
                // ===== ĐÓNG KẾT NỐI =====
                // Khi thoát vòng lặp (người dùng nhấn Enter), đóng kết nối
                client.Close();
            }
            catch (Exception e)
            {
                // ===== XỬ LÝ LỖI =====
                // Có thể xảy ra lỗi khi:
                //   - Server không chạy
                //   - Không kết nối được mạng
                //   - Server ngắt kết nối đột ngột
                Console.WriteLine("Exception: {0}", e.Message);
            }

        }//end ConnectServer

        // ===== HÀM MAIN - ĐIỂM BẮT ĐẦU CHƯƠNG TRÌNH =====
        static void Main(string[] args)
        {
            // ===== CẤU HÌNH KẾT NỐI =====
            
            // Server: 127.0.0.1 = localhost (kết nối đến server trên máy local)
            // Nếu server ở máy khác, thay bằng địa chỉ IP của máy đó
            string server = "127.0.0.1";

            // Port: 13000 = cổng mà server đang lắng nghe
            // PHẢI TRÙNG với port của server, nếu không sẽ không kết nối được
            int port = 13000;

            // ===== KẾT NỐI ĐẾN SERVER =====
            // Gọi hàm ConnectServer để bắt đầu giao tiếp
            ConnectServer(server, port);

        }//end Main

    }//end Program
}

