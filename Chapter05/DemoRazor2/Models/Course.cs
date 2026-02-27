using System.ComponentModel.DataAnnotations;

namespace DemoRazor2.Models;

public class Course
{
    [Key]                 // dùng ID làm PK
    public int CourseID { get; set; }

    [Required, StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Range(0, 10)]
    public int Credits { get; set; }
}
