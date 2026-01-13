// Import namespace System.Net để sử dụng các lớp làm việc với mạng
// (WebRequest, HttpWebResponse, v.v.)
using System.Net;

namespace WebRequestDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // ===== BƯỚC 1: TẠO YÊU CẦU (REQUEST) =====
            // Tạo một yêu cầu HTTP để lấy dữ liệu từ URL
            // Giống như khi bạn gõ địa chỉ web vào trình duyệt
            WebRequest request = WebRequest.Create("http://www.contoso.com/default.html");

            // ===== BƯỚC 2: CÀI ĐẶT XÁC THỰC (NẾU CẦN) =====
            // Nếu server yêu cầu đăng nhập, sẽ dùng thông tin xác thực
            // DefaultCredentials = thông tin đăng nhập mặc định của Windows
            request.Credentials = CredentialCache.DefaultCredentials;

            // ===== BƯỚC 3: GỬI YÊU CẦU VÀ NHẬN PHẢN HỒI (RESPONSE) =====
            // GetResponse() gửi yêu cầu đến server và đợi nhận kết quả
            // Kết quả trả về là một đối tượng HttpWebResponse
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // ===== BƯỚC 4: KIỂM TRA TRẠNG THÁI PHẢN HỒI =====
            // StatusDescription cho biết kết quả: "OK" = thành công, "Not Found" = không tìm thấy, v.v.
            Console.WriteLine("Status: " + response.StatusDescription);

            // In ra một dòng kẻ gồm 50 dấu sao để phân cách
            Console.WriteLine(new string('*', 50));

            // ===== BƯỚC 5: LẤY DỮ LIỆU TỪ SERVER =====
            // GetResponseStream() trả về một luồng (Stream) chứa dữ liệu HTML/nội dung từ server
            // Stream giống như "ống nước" chảy dữ liệu từ server về máy bạn
            Stream dataStream = response.GetResponseStream();

            // ===== BƯỚC 6: ĐỌC DỮ LIỆU TỪ STREAM =====
            // StreamReader giúp đọc dữ liệu từ Stream dễ dàng hơn
            // (chuyển từ dạng bytes thành dạng text/string)
            StreamReader reader = new StreamReader(dataStream);

            // Đọc TẤT CẢ nội dung từ đầu đến cuối (HTML của trang web)
            // ReadToEnd() đọc hết dữ liệu và trả về dưới dạng string
            string responseFromServer = reader.ReadToEnd();

            // ===== BƯỚC 7: HIỂN THỊ NỘI DUNG =====
            // In ra nội dung HTML nhận được từ server
            Console.WriteLine(responseFromServer);
            Console.WriteLine(new string('*', 50));

            // ===== BƯỚC 8: ĐÓNG VÀ DỌN DẸP TÀI NGUYÊN =====
            // Quan trọng: Phải đóng các stream và response để giải phóng bộ nhớ
            // Nếu không đóng, có thể gây rò rỉ bộ nhớ (memory leak)
            
            // Đóng StreamReader
            reader.Close();

            // Đóng Stream chứa dữ liệu
            dataStream.Close();

            // Đóng kết nối HTTP
            response.Close();
        }
    }
}
