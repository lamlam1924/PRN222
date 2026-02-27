using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace TravelManagementApp.Filters;

public class SessionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;
        
        // Get customer info from Cookie Authentication claims if authenticated
        if (user?.Identity?.IsAuthenticated == true)
        {
            var customerId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var customerName = user.FindFirst(ClaimTypes.Name)?.Value;
            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            
            // Gửi customer info vào ViewBag cho tất cả views
            if (context.Controller is Controller controller)
            {
                controller.ViewBag.CustomerID = customerId != null ? int.Parse(customerId) : (int?)null;
                controller.ViewBag.CustomerName = customerName;
                controller.ViewBag.Role = role;
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}