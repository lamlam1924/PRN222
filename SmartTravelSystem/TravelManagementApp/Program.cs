using Microsoft.EntityFrameworkCore;
using TravelDataAccess.Models;
using TravelManagementApp.Filters;
using Microsoft.AspNetCore.Authentication.Cookies;

// ============================================
// PHẦN 1: CẤU HÌNH SERVICES (Dependency Injection)
// ============================================
var builder = WebApplication.CreateBuilder(args);

// Đăng ký MVC Controllers và Views
// Options.Filters: Thêm các filter chạy trước mỗi request
builder.Services.AddControllersWithViews(options =>
{
    // AuthFilter: Kiểm tra authentication VÀ đưa thông tin user vào ViewBag
    // SessionFilter đã XÓA vì duplicate với AuthFilter
    options.Filters.Add<AuthFilter>();
});


// Đăng ký Entity Framework DbContext
// Connection string lấy từ appsettings.json
builder.Services.AddDbContext<TravelContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TravelDB")));

// Cấu hình Cookie Authentication
// Đây là cơ chế lưu thông tin đăng nhập vào cookie của browser
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Đường dẫn redirect khi chưa đăng nhập
        options.LoginPath = "/Login/Index";
        
        // Đường dẫn logout
        options.LogoutPath = "/Login/Logout";
        
        // Đường dẫn khi bị từ chối quyền truy cập
        options.AccessDeniedPath = "/Login/Index";
        
        // Cookie hết hạn sau 1 giờ
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        
        // SlidingExpiration = true: Tự động gia hạn thời gian cookie mỗi khi có request
        // Ví dụ: Cookie còn 10 phút, user load trang -> reset lại 1 giờ
        options.SlidingExpiration = true;
        
        // HttpOnly = true: Cookie chỉ truy cập qua HTTP, không qua JavaScript
        // -> Bảo mật cao hơn, tránh XSS attack
        options.Cookie.HttpOnly = true;
        
        // SecurePolicy: Cookie chỉ gửi qua HTTPS (trong production)
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        
        // SameSite: Bảo vệ khỏi CSRF attack
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// ============================================
// PHẦN 2: CẤU HÌNH MIDDLEWARE PIPELINE
// ============================================
// Middleware chạy theo thứ tự: Request đi từ trên xuống, Response đi từ dưới lên
var app = builder.Build();

// Nếu không phải môi trường Development
if (!app.Environment.IsDevelopment())
{
    // Xử lý lỗi và redirect về trang Error
    app.UseExceptionHandler("/Home/Error");
    
    // HSTS: Bắt buộc browser luôn dùng HTTPS
    app.UseHsts();
}

// Tự động redirect HTTP -> HTTPS
app.UseHttpsRedirection();

// Cho phép serve static files (CSS, JS, images) từ wwwroot folder
app.UseStaticFiles();

// Enable routing - map URL vào Controller/Action tương ứng
app.UseRouting();

// ⚠️ THỨ TỰ QUAN TRỌNG:
// UseAuthentication phải đứng TRƯỚC UseAuthorization

// Authentication: Xác định user là ai (đọc cookie, tạo User object)
app.UseAuthentication();

// Authorization: Kiểm tra user có quyền truy cập không (dựa vào Role)
app.UseAuthorization();

// Map route mặc định: /{controller}/{action}/{id?}
// Ví dụ: /Home/Index, /Trip/Details/5
// Mặc định: controller=Home, action=Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Chạy ứng dụng và lắng nghe HTTP requests
app.Run();