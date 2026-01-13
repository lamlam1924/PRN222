// ===== IMPORT CÁC NAMESPACE CẦN THIẾT =====
using System.Net;             // Làm việc với địa chỉ IP
using System.Net.Sockets;     // Làm việc với Socket và UDP
using System.Text;            // Encoding để chuyển đổi string ↔ bytes

namespace UDPClientApp
{
    internal class Program
    {
        // ===== HÀM KẾT NỐI VÀ GỬI DỮ LIỆU ĐẾN SERVER =====
        static void ConnectServer(string host, int port)
        {
            // ===== TẠO UDP CLIENT =====
            // UDP "connectionless" = KHÔNG cần thiết lập kết nối trước
            // Khác với TCP phải connect trước khi gửi
            UdpClient client = new UdpClient();
            
            // Parse địa chỉ IP từ string
            IPAddress address = IPAddress.Parse(host);
            
            // ===== TẠO ENDPOINT =====
            // IPEndPoint chỉ định địa chỉ đích (server) để gửi dữ liệu đến
            IPEndPoint remoteEndpoint = new IPEndPoint(address, port);

            string message;
            int count = 0;       // Đếm số tin nhắn đã gửi
            bool done = false;   // Flag để kiểm soát vòng lặp
            Console.Title = "UDP Client";

            try
            {
                Console.WriteLine(new string('*', 40));
                
                // ===== CONNECT (OPTIONAL TRONG UDP) =====
                // UDP.Connect() KHÔNG thực sự tạo kết nối
                // Chỉ "ghi nhớ" địa chỉ đích để không phải chỉ định mỗi lần Send()
                // 
                // Với Connect():
                //   - client.Send(data, length)  ← không cần chỉ định địa chỉ
                // Không Connect():
                //   - client.Send(data, length, endpoint)  ← phải chỉ định địa chỉ mỗi lần
                client.Connect(remoteEndpoint);

                // ===== VÒNG LẶP GỬI TIN NHẮN =====
                while (!done)
                {
                    // ===== TẠO TIN NHẮN =====
                    // {++count:D2} = tăng count và format thành 2 chữ số (01, 02, 03...)
                    message = $"Message {++count:D2}";
                    
                    // Chuyển string → bytes
                    byte[] sendBytes = Encoding.ASCII.GetBytes(message);
                    
                    // ===== GỬI DỮ LIỆU =====
                    // Send() gửi datagram (gói tin) đến server
                    // 
                    // QUAN TRỌNG - Đặc điểm UDP:
                    //   - "Fire and forget" = gửi xong không quan tâm có đến không
                    //   - KHÔNG đảm bảo: dữ liệu có thể bị mất, trùng lặp, đến sai thứ tự
                    //   - KHÔNG có acknowledgment (xác nhận nhận được)
                    //   - Nhanh hơn TCP vì không có overhead của connection
                    client.Send(sendBytes, sendBytes.Length);
                    Console.WriteLine($"Sent: {message}");

                    // ===== TẠM DỪNG 2 GIÂY =====
                    // Thread.Sleep() dừng thread hiện tại trong X milliseconds
                    // 2000ms = 2 giây
                    // Mục đích: tránh gửi quá nhanh, dễ quan sát output
                    Thread.Sleep(2000);

                    // ===== KIỂM TRA ĐIỀU KIỆN DỪNG =====
                    // Sau khi gửi 10 tin nhắn thì dừng
                    if (count == 10) //broadcast 10 messages
                    {
                        done = true;
                        Console.WriteLine("Done.");
                    }
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
                client.Close();
            }

        }//end ConnectServer

        // ===== HÀM MAIN - ĐIỂM BẮT ĐẦU CHƯƠNG TRÌNH =====
        static void Main(string[] args)
        {
            // ===== CẤU HÌNH =====
            // Host: 127.0.0.1 = localhost (gửi đến server trên máy local)
            string host = "127.0.0.1";
            
            // Port: 11000 = port mà server đang lắng nghe
            // PHẢI TRÙNG với port của server
            int port = 11000;
            
            // ===== GỬI TIN NHẮN ĐẾN SERVER =====
            ConnectServer(host, port);
            
            // Chờ người dùng nhấn phím trước khi thoát
            Console.Read();

        }//end Main

    }//end Program
}
