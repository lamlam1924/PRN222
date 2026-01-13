// Import namespace System.Net để sử dụng class Dns
using System.Net;

namespace DNSDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // ===== PHẦN 1: TRA CỨU DNS TỪ TÊN MIỀN → ĐỊA CHỈ IP =====
            
            // In dòng phân cách (30 dấu sao)
            Console.WriteLine(new string('*', 30));

            // DNS là gì?
            //   - DNS = Domain Name System (Hệ thống tên miền)
            //   - Chuyển đổi tên miền (vd: www.google.com) thành địa chỉ IP (vd: 172.217.14.196)
            //   - Giống như "danh bạ điện thoại" của Internet
            
            // GetHostEntry() - Tra cứu thông tin DNS từ tên miền
            // Trả về: 
            //   - HostName: tên máy chủ chính thức
            //   - AddressList: danh sách các địa chỉ IP của domain
            var domainEntry = Dns.GetHostEntry("www.contoso.com");

            // In ra tên máy chủ chính thức (canonical hostname)
            Console.WriteLine(domainEntry.HostName);

            // Duyệt qua TẤT CẢ các địa chỉ IP của domain
            // Lý do có nhiều IP:
            //   - Load balancing (cân bằng tải)
            //   - Dự phòng (nếu 1 server down, còn server khác)
            //   - IPv4 và IPv6
            foreach (var ip in domainEntry.AddressList)
            {
                // In ra từng địa chỉ IP
                Console.WriteLine(ip);
            }

            // ===== PHẦN 2: TRA CỨU NGƯỢC (REVERSE DNS) - TỪ IP → TÊN MIỀN =====
            
            Console.WriteLine(new string('*', 30));

            // 127.0.0.1 = địa chỉ IP đặc biệt
            //   - Gọi là "localhost" hoặc "loopback"
            //   - Luôn trỏ về CHÍNH MÁY TÍNH của bạn
            //   - Dùng để test ứng dụng trên máy local
            
            // Tra cứu ngược: từ địa chỉ IP → lấy thông tin hostname
            var domainEntryByAddress = Dns.GetHostEntry("127.0.0.1");

            // In ra tên máy tính của bạn
            // Kết quả thường là: tên máy tính Windows của bạn
            Console.WriteLine(domainEntryByAddress.HostName);

            // In ra tất cả địa chỉ IP của máy local
            // Thường có cả IPv4 (127.0.0.1) và IPv6 (::1)
            foreach (var ip in domainEntryByAddress.AddressList)
            {
                Console.WriteLine(ip);
            }

            // Dừng chương trình cho đến khi nhấn Enter
            Console.ReadLine();
        }
    }
}
