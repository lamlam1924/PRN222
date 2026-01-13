// ===== IMPORT CÁC NAMESPACE CẦN THIẾT =====
using System.Net;             // Làm việc với địa chỉ IP
using System.Net.Sockets;     // Làm việc với Socket và UDP
using System.Text;            // Encoding để chuyển đổi string ↔ bytes

namespace UDPServerApp
{
    internal class Program
    {
        // ===== KHAI BÁO HẰNG SỐ =====
        // Port để lắng nghe (client phải gửi đến đúng port này)
        const int listenPort = 11000;
        
        // Host/IP address để lắng nghe
        const string host = "127.0.0.1";

        // ===== HÀM LẮNG NGHE VÀ NHẬN DỮ LIỆU UDP =====
        private static void StartListener()
        {
            string message;
            
            // ===== TẠO UDP CLIENT (SERVER) =====
            // UDP KHÔNG phân biệt "Server" và "Client" rõ ràng như TCP
            // UdpClient vừa có thể gửi vừa có thể nhận
            // Tham số: listenPort = port để lắng nghe
            UdpClient listener = new UdpClient(listenPort);

            // Parse địa chỉ IP từ string
            IPAddress address = IPAddress.Parse(host);

            // ===== TẠO ENDPOINT =====
            // IPEndPoint = kết hợp IP + Port
            // Dùng để xác định "địa chỉ" của sender/receiver
            IPEndPoint remoteEndpoint = new IPEndPoint(address, listenPort);
            
            Console.Title = "UDP Server";
            Console.WriteLine(new string('*', 40));

            try
            {
                // ===== VÒNG LẶP VÔ HẠN - NHẬN DỮ LIỆU =====
                // Server luôn chạy và chờ nhận dữ liệu
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast");

                    // ===== NHẬN DỮ LIỆU TỪ CLIENT =====
                    // Receive() chờ (block) cho đến khi có dữ liệu đến
                    // "ref remoteEndpoint" = truyền tham chiếu
                    //   → Sau khi Receive(), remoteEndpoint sẽ chứa địa chỉ của sender
                    // 
                    // QUAN TRỌNG - Khác biệt với TCP:
                    //   - UDP: Không cần AcceptTcpClient(), không cần Thread cho mỗi client
                    //   - UDP: Chỉ cần Receive() là nhận được dữ liệu từ BẤT KỲ client nào
                    byte[] bytes = listener.Receive(ref remoteEndpoint);
                    
                    // Chuyển đổi bytes thành string
                    message = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    
                    // In ra tin nhắn + địa chỉ của sender
                    // remoteEndpoint tự động chứa IP:Port của người gửi
                    Console.WriteLine($"Received broadcast from {remoteEndpoint}:{message}");
                }
            }
            catch (SocketException e)
            {
                // Bắt lỗi liên quan đến socket
                Console.WriteLine(e.Message);
            }
            finally
            {
                // ===== ĐÓNG UDP CLIENT =====
                // Giải phóng tài nguyên
                listener.Close();
            }

        }//end StartListener

        // ===== HÀM MAIN - ĐIỂM BẮT ĐẦU CHƯƠNG TRÌNH =====
        public static void Main()
        {
            // ===== TẠO THREAD ĐỂ CHẠY SERVER =====
            // Tạo thread riêng để chạy StartListener()
            // (Trong ví dụ này không thực sự cần thread vì chỉ có 1 listener,
            //  nhưng minh họa cách sử dụng thread)
            Thread thread = new Thread(new ThreadStart(StartListener));
            thread.Start();

        }//end Main

    }//end Program
}
