using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using TravelDataAccess.Models;

namespace TravelManagementApp.Controllers;

/// <summary>
/// Controller xử lý đăng nhập và đăng xuất
/// Sử dụng Cookie Authentication để lưu thông tin user
/// </summary>
public class LoginController : Controller
{
    // Database context - dùng để truy vấn dữ liệu
    private readonly TravelContext _context;

    // Constructor - ASP.NET Core tự động inject TravelContext vào
    public LoginController(TravelContext context)
    {
        _context = context;
    }

    // GET: /Login/Index - Hiển thị trang đăng nhập
    public IActionResult Index() => View();

    // POST: /Login/Index - Xử lý khi user submit form đăng nhập
    [HttpPost]
    public async Task<IActionResult> Index(string code, string password)
    {
        // Bước 1: Tìm user trong database theo code và password
        var user = _context.Customers
            .FirstOrDefault(x => x.Code == code && x.Password == password);

        // Bước 2: Nếu không tìm thấy user -> đăng nhập thất bại
        if (user == null)
        {
            ViewBag.Error = "Sai mã hoặc mật khẩu.";
            return View();
        }

        // Bước 3: Tạo Claims - các thông tin về user để lưu vào cookie
        // Claims giống như key-value pairs chứa thông tin user
        var claims = new List<Claim>
        {
            // ClaimTypes chuẩn - ASP.NET Core tự động nhận diện
            new Claim(ClaimTypes.NameIdentifier, user.CustomerID.ToString()), // ID của user
            new Claim(ClaimTypes.Name, user.FullName),                        // Tên đầy đủ
            new Claim(ClaimTypes.Role, user.Role),                            // Role: Admin, Customer, etc.
            
            // Custom claims - chỉ thêm thông tin KHÔNG CÓ trong ClaimTypes chuẩn
            new Claim("Code", user.Code)  // Code không có trong ClaimTypes chuẩn nên phải tự định nghĩa
        };
      
        // Bước 4: Tạo ClaimsIdentity - đại diện cho danh tính của user
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        // Bước 5: Cấu hình thuộc tính của authentication cookie
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,  // Cookie tồn tại ngay cả khi đóng browser (Remember me)
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)  // Cookie hết hạn sau 1 giờ
        };

        // Bước 6: Đăng nhập user - tạo cookie và gửi về browser
        // SignInAsync sẽ:
        // - Mã hóa thông tin claims
        // - Tạo cookie với tên ".AspNetCore.Cookies"
        // - Gửi cookie về browser qua HTTP response header
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        // Bước 7: Redirect về trang chủ sau khi đăng nhập thành công
        return RedirectToAction("Index", "Home");
    }

    // GET: /Login/Logout - Đăng xuất user
    public async Task<IActionResult> Logout()
    {
        // Xóa authentication cookie -> user sẽ không còn đăng nhập
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        
        // Redirect về trang login
        return RedirectToAction("Index");
    }
}
