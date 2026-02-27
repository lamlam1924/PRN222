using System.ComponentModel.DataAnnotations;

namespace DemoRazor2.Models;

public class Student
{
    public int ID { get; set; }

    [Required, StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string FirstMidName { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime EnrollmentDate { get; set; }
}
