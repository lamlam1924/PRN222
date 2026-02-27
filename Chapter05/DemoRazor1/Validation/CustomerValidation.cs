using System.ComponentModel.DataAnnotations;

namespace DemoRazor1.Validation;

public class CustomerValidation: ValidationAttribute
{
    public CustomerValidation()
    {
        ErrorMessage = $"The year of birth cannot be greater than current year " +
                       $"({DateTime.Now.Year}).";
    }
    public override bool IsValid(object? value)
    {
        if (value == null)
            return false;
        int number = Int32.Parse(value.ToString());
        return (number < DateTime.Now.Year && number>1900);

    }
}
