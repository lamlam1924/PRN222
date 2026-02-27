using Microsoft.AspNetCore.Mvc;

namespace TravelManagementApp.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        // Authentication is handled by AuthFilter
        return View();
    }
}
