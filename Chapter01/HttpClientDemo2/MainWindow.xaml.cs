// ===== IMPORT CÁC THÀNH PHẦN CẦN THIẾT =====
using System.Net.Http;        // Để sử dụng HttpClient (gửi HTTP request)
using System.Text;            // Xử lý chuỗi và encoding
using System.Windows;         // Thành phần cơ bản của WPF (Window, MessageBox...)
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HttpClientDemo2
{
    /// <summary>
    /// Logic xử lý cho cửa sổ chính (MainWindow.xaml)
    /// </summary>
    public partial class MainWindow : Window
    {
        // ===== HÀM KHỞI TẠO (CONSTRUCTOR) =====
        // Được gọi khi tạo cửa sổ mới
        public MainWindow()
        {
            // InitializeComponent() khởi tạo tất cả controls từ file XAML
            // (buttons, textboxes, labels... được định nghĩa trong MainWindow.xaml)
            InitializeComponent();
        }

        // ===== KHAI BÁO HttpClient =====
        // readonly = chỉ gán 1 lần, không thể thay đổi
        // Tạo 1 HttpClient dùng chung cho toàn bộ window (tái sử dụng kết nối)
        readonly HttpClient client = new HttpClient();

        // ===== SỰ KIỆN: KHI CLICK NÚT CLOSE =====
        // Lambda expression (=>) - cách viết ngắn gọn của hàm
        // Close() = đóng cửa sổ hiện tại
        private void btnClose_Click(object sender, RoutedEventArgs e) => Close();

        // ===== SỰ KIỆN: KHI CLICK NÚT CLEAR =====
        // Xóa sạch nội dung trong TextBox hiển thị HTML
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            // txtContent = tên của TextBox trong file XAML
            // string.Empty = chuỗi rỗng (giống như "")
            txtContent.Text = string.Empty;
        }

        // ===== SỰ KIỆN: KHI CLICK NÚT VIEW HTML =====
        // "async" = hàm bất đồng bộ (không làm "đơ" giao diện khi đợi dữ liệu)
        // Quan trọng trong ứng dụng GUI: tránh UI bị freeze khi chờ HTTP response
        private async void btnViewHTML_Click(object sender, RoutedEventArgs e)
        {
            // Lấy URL từ TextBox mà người dùng nhập vào
            // txtURL = tên của TextBox chứa URL trong file XAML
            string uri = txtURL.Text;

            // ===== XỬ LÝ YÊU CẦU HTTP VỚI TRY-CATCH =====
            // Try-catch bắt lỗi để tránh app bị crash
            try
            {
                // ===== GỬI REQUEST VÀ ĐỌC KẾT QUẢ =====
                // GetStringAsync() = gửi GET request và đọc kết quả thành string (1 bước)
                // Đơn giản hơn so với GetAsync() + ReadAsStringAsync()
                // "await" = đợi kết quả mà KHÔNG làm đơ giao diện
                string responseBody = await client.GetStringAsync(uri);

                // Trim() = xóa khoảng trắng thừa ở đầu/cuối chuỗi
                // Hiển thị HTML trong TextBox
                txtContent.Text = responseBody.Trim();
            }
            catch (HttpRequestException ex) // Bắt lỗi khi có vấn đề với HTTP request
            {
                // ===== HIỂN THỊ LỖI CHO NGƯỜI DÙNG =====
                // MessageBox.Show() = hiển thị hộp thoại thông báo
                // Thông báo lỗi thay vì để app crash
                MessageBox.Show($"Message : {ex.Message}");
            }
        }
    }
}
