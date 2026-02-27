// Import static class Console để có thể dùng WriteLine và ReadLine trực tiếp
// (không cần viết Console.WriteLine)
using static System.Console;

namespace URIsDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // ===== TẠO ĐỐI TƯỢNG URI =====
            // URI là "địa chỉ" của một tài nguyên trên Internet
            // URI gồm các phần: http://www.domain.com:80/info?id=123#fragment
            //   - http:// = giao thức (protocol)
            //   - www.domain.com = tên máy chủ (host/domain)
            //   - :80 = cổng kết nối (port)
            //   - /info = đường dẫn (path)
            //   - ?id=123 = tham số truy vấn (query string)
            //   - #fragment = phần neo/đánh dấu (fragment/anchor)
            Uri info = new Uri("http://www.domain.com:80/info?id=123#fragment");

            // Tạo một URI khác đơn giản hơn (không có port, query, fragment)
            Uri page = new Uri("http://www.domain.com/info/page.html");

            // ===== HIỂN THỊ CÁC THÀNH PHẦN CỦA URI =====
            
            // Lấy tên máy chủ (Host) - kết quả: www.domain.com
            WriteLine($"Host: {info.Host}");

            // Lấy số cổng (Port) - kết quả: 80
            WriteLine($"Port: {info.Port}");

            // Lấy đường dẫn + tham số truy vấn - kết quả: /info?id=123
            WriteLine($"PathAndQuery: {info.PathAndQuery}");

            // Chỉ lấy phần tham số truy vấn - kết quả: ?id=123
            WriteLine($"Query: {info.Query}");

            // Lấy phần fragment (neo) - kết quả: #fragment
            WriteLine($"Fragment: {info.Fragment}");

            // Lấy port mặc định của HTTP (port 80)
            // Vì page không chỉ định port nên sẽ dùng port mặc định
            WriteLine($"Default HTTP port: {page.Port}");

            // ===== SO SÁNH VÀ CHUYỂN ĐỔI URI =====
            
            // Kiểm tra xem info có phải là "base" (gốc) của page không
            // Nghĩa là: page có bắt đầu bằng info không?
            // Kết quả: True vì cả 2 đều bắt đầu bằng http://www.domain.com
            WriteLine($"IsBaseOf: {info.IsBaseOf(page)}");

            // Tạo URI tương đối (relative) từ info đến page
            // Nghĩa là: từ vị trí info, cần đi đường nào để đến page?
            Uri relative = info.MakeRelativeUri(page);

            // Kiểm tra xem relative có phải là URI tuyệt đối không
            // Kết quả: False vì nó chỉ là đường dẫn tương đối
            WriteLine($"IsAbsoluteUri: {relative.IsAbsoluteUri}");

            // In ra URI tương đối - kết quả: info/page.html
            // (từ /info -> /info/page.html)
            WriteLine($"RelativeUri: {relative.ToString()}");

            // Dừng chương trình cho đến khi người dùng nhấn Enter
            ReadLine();
        }
    }
}
