using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace TravelManagementApp.Filters;

public class AuthFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var path = context.HttpContext.Request.Path.Value ?? "";

        // Skip authentication for Login controller
        if (path.StartsWith("/Login", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        // Check if user is authenticated via Cookie Authentication
        var user = context.HttpContext.User;
        
        if (user?.Identity?.IsAuthenticated != true)
        {
            // Not authenticated, redirect to login
            context.Result = new RedirectToActionResult("Index", "Login", null);
            return;
        }

        // Get customer info from claims
        var customerId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var customerName = user.FindFirst(ClaimTypes.Name)?.Value;
        var role = user.FindFirst(ClaimTypes.Role)?.Value;

        // Pass to ViewBag for all views
        if (context.Controller is Controller controller)
        {
            controller.ViewBag.CustomerID = customerId != null ? int.Parse(customerId) : (int?)null;
            controller.ViewBag.CustomerName = customerName;
            controller.ViewBag.Role = role;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}