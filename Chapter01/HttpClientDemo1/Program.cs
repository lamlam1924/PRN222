namespace HttpClientDemo1
{
    internal class Program
    {
        // ===== KHAI BÁO HttpClient =====
        // HttpClient nên được tạo 1 LẦN DUY NHẤT cho toàn bộ ứng dụng
        // Lý do: 
        //   - Tái sử dụng kết nối (connection pooling) → hiệu suất tốt hơn
        //   - Tránh hết socket khi tạo quá nhiều HttpClient
        // 
        // Từ khóa:
        //   - static: biến này dùng chung cho cả class, không thuộc về 1 object cụ thể
        //   - readonly: chỉ có thể gán giá trị 1 lần, không thể thay đổi sau đó
        static readonly HttpClient client = new HttpClient();

        // ===== HÀM MAIN BẤT ĐỒNG BỘ (ASYNC) =====
        // "async" = hàm này chạy bất đồng bộ (asynchronous)
        // "Task" = kiểu trả về của hàm async (giống void nhưng cho async)
        // 
        // Async là gì?
        //   - Khi gửi request HTTP, phải đợi server trả về (có thể mất vài giây)
        //   - Thay vì "đứng chờ" (blocking), async cho phép chương trình làm việc khác
        //   - Khi dữ liệu về, chương trình sẽ tiếp tục xử lý
        static async Task Main()
        {
            // URL của trang web muốn truy cập
            string uri = "http://www.contoso.com/";

            // ===== XỬ LÝ LỖI VỚI TRY-CATCH =====
            // Khi làm việc với mạng, có thể gặp nhiều lỗi:
            //   - Không có internet
            //   - Server không phản hồi
            //   - URL sai
            // → Dùng try-catch để "bắt" và xử lý lỗi
            try
            {
                // ===== GỬI REQUEST VÀ NHẬN RESPONSE =====
                // GetAsync() = gửi HTTP GET request (lấy dữ liệu)
                // "await" = đợi kết quả trả về (nhưng không block chương trình)
                // 
                // So sánh với WebRequest:
                //   - HttpClient: hiện đại, async, dễ dùng ✅
                //   - WebRequest: cũ, sync, phức tạp hơn ❌
                HttpResponseMessage response = await client.GetAsync(uri);

                // ===== KIỂM TRA TRẠNG THÁI PHẢN HỒI =====
                // EnsureSuccessStatusCode() kiểm tra xem request có thành công không
                // Nếu status code không phải 200-299 (thành công), sẽ throw exception
                // Ví dụ: 404 Not Found, 500 Server Error → throw exception
                response.EnsureSuccessStatusCode();

                // ===== ĐỌC NỘI DUNG PHẢN HỒI =====
                // ReadAsStringAsync() đọc nội dung HTML/text từ response
                // "await" = đợi quá trình đọc hoàn tất
                string responseBody = await response.Content.ReadAsStringAsync();

                // In nội dung ra console
                Console.WriteLine(responseBody);

            }
            catch (HttpRequestException e) // Bắt lỗi liên quan đến HTTP request
            {
                // ===== XỬ LÝ KHI CÓ LỖI =====
                Console.WriteLine("\nException Caught!"); // Thông báo bắt được lỗi
                Console.WriteLine("Message : {0} ", e.Message); // In chi tiết lỗi
            }

            // ===== LƯU Ý QUAN TRỌNG =====
            // Khác với WebRequest, HttpClient KHÔNG cần Close()
            // Vì HttpClient được tái sử dụng, chỉ dispose khi tắt app
        }
    }
}
