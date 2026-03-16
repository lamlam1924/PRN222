using Microsoft.AspNetCore.Mvc;
using SU26_PRN222_Healthcare.CareBusiness.Entities;
using SU26_PRN222_Healthcare.CareRepositories;

namespace SU26_PRN222_Healthcare.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly ISessionRepository _sessionRepo;

        public AccountController(IUserRepository userRepo, ISessionRepository sessionRepo)
        {
            _userRepo = userRepo;
            _sessionRepo = sessionRepo;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, redirect based on role
            var sessionId = HttpContext.Session.GetString("SessionID");
            if (!string.IsNullOrEmpty(sessionId))
            {
                var session = _sessionRepo.GetBySessionId(sessionId);
                if (session != null && session.ExpiresAt > DateTime.Now)
                {
                    return RedirectByRole(session.Role);
                }
            }
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Email and password are required.";
                return View();
            }

            var user = _userRepo.GetByEmail(email);
            if (user == null || user.Password != password)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            // Create session record
            var session = new Session
            {
                SessionID = Guid.NewGuid().ToString(),
                UserID = user.ID,
                Role = user.Role,
                ExpiresAt = DateTime.Now.AddHours(2)
            };
            _sessionRepo.Create(session);

            // Store session ID in HTTP session cookie
            HttpContext.Session.SetString("SessionID", session.SessionID);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetInt32("UserID", user.ID);

            return RedirectByRole(user.Role);
        }

        // GET: /Account/Logout
        [HttpGet]
        public IActionResult Logout()
        {
            var sessionId = HttpContext.Session.GetString("SessionID");
            if (!string.IsNullOrEmpty(sessionId))
            {
                _sessionRepo.Delete(sessionId);
            }
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(string fullName, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "All fields are required.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            var existing = _userRepo.GetByEmail(email);
            if (existing != null)
            {
                ViewBag.Error = "An account with this email already exists.";
                return View();
            }

            var user = new User
            {
                FullName = fullName,
                Email = email,
                Password = password,
                Role = "Patient",
                CreatedAt = DateTime.Now
            };
            _userRepo.Add(user);
            TempData["Success"] = "Account registered successfully. Please login.";
            return RedirectToAction("Login");
        }

        private IActionResult RedirectByRole(string role)
        {
            if (role == "Admin")
                return RedirectToAction("Index", "Dashboard");
            else
                return RedirectToAction("Search", "Doctor");
        }
    }
}
