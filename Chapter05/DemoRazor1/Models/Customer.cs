using System.ComponentModel.DataAnnotations;
using DemoRazor1.Validation;

namespace DemoRazor1.Models;

public class Customer
{
    [Required(ErrorMessage = "Customer name is required!")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Name length must be 3-20")]
    [Display(Name = "Customer name")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Customer email is required!")]
    [EmailAddress(ErrorMessage = "Invalid email!")]
    [Display(Name = "Customer email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Year of birth is required!")]
    [Display(Name = "Year of birth")]
    [CustomerValidation]
    public int? YearOfBirth { get; set; }
}
