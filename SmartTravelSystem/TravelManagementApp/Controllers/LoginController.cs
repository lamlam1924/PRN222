using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using TravelDataAccess.Models;

namespace TravelManagementApp.Controllers;

public class LoginController : Controller
{
    private readonly TravelContext _context;

    public LoginController(TravelContext context)
    {
        _context = context;
    }

    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> Index(string code, string password)
    {
        var user = _context.Customers
            .FirstOrDefault(x => x.Code == code && x.Password == password);

        if (user == null)
        {
            ViewBag.Error = "Sai mã hoặc mật khẩu.";
            return View();
        }

        // Create claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.CustomerID.ToString()),
            new Claim("CustomerId", user.CustomerID.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim("CustomerName", user.FullName),
            new Claim("Code", user.Code),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("Role", user.Role)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true, // Remember me
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
        };

        // Sign in user
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index");
    }
}
