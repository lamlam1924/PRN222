using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace TravelManagementApp.Filters;

/// <summary>
/// Authentication Filter - Kiểm tra user đã đăng nhập chưa
/// Filter này chạy TRƯỚC mỗi action trong controller
/// </summary>
public class AuthFilter : IActionFilter
{
    // OnActionExecuting: Chạy TRƯỚC khi action thực thi
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Lấy đường dẫn hiện tại (ví dụ: /Home/Index, /Login/Index)
        var path = context.HttpContext.Request.Path.Value ?? "";

        // Bỏ qua kiểm tra authentication cho trang Login
        // Nếu không có dòng này -> vòng lặp vô hạn redirect
        if (path.StartsWith("/Login", StringComparison.OrdinalIgnoreCase))
        {
            return;  // Cho phép truy cập không cần đăng nhập
        }

        // Lấy thông tin user từ HttpContext (đã được set bởi middleware Authentication)
        // User object được tạo tự động từ cookie nếu user đã đăng nhập
        var user = context.HttpContext.User;
        
        // Kiểm tra user đã authenticated (đã đăng nhập) chưa?
        if (user?.Identity?.IsAuthenticated != true)
        {
            // Chưa đăng nhập -> Redirect về trang login
            // Set context.Result sẽ dừng pipeline và không chạy action nữa
            context.Result = new RedirectToActionResult("Index", "Login", null);
            return;
        }

        // Đã đăng nhập -> Lấy thông tin user từ claims
        // Claims đã được lưu trong cookie khi login (xem LoginController)
        var customerId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;  // Dùng ClaimTypes chuẩn
        var customerName = user.FindFirst(ClaimTypes.Name)?.Value;          // Dùng ClaimTypes chuẩn
        var role = user.FindFirst(ClaimTypes.Role)?.Value;                  // Dùng ClaimTypes chuẩn

        // Đưa thông tin vào ViewBag để tất cả View có thể dùng
        // Ví dụ: Hiển thị tên user trên layout, check quyền trong view
        if (context.Controller is Controller controller)
        {
            controller.ViewBag.CustomerID = customerId != null ? int.Parse(customerId) : (int?)null;
            controller.ViewBag.CustomerName = customerName;
            controller.ViewBag.Role = role;
        }
    }

    // OnActionExecuted: Chạy SAU khi action thực thi xong
    // Để trống vì không cần xử lý gì - nhưng BẮT BUỘC phải có do IActionFilter interface yêu cầu
    public void OnActionExecuted(ActionExecutedContext context) { }
}