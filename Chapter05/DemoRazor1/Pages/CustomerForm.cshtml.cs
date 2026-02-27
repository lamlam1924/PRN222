using DemoRazor1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoRazor1.Pages;

public class CustomerFormModel : PageModel
{
    [BindProperty]
    public Customer CustomerInfo { get; set; } = new();

    public string? Message { get; set; }

    public void OnGet()
    {
        
        if (ModelState.IsValid)
        {
            Message = "Information is OK";
            // nếu muốn clear form sau submit:
            // ModelState.Clear();
        }
        else
        {
            Message = "Error on input data.";
        }
    }

}
