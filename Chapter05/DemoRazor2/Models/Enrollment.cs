namespace DemoRazor2.Models;

public class Enrollment
{
    public int EnrollmentID { get; set; }

    public int StudentID { get; set; }
    public int CourseID { get; set; }

    // Nav (để scaffold dễ hiển thị)
    public Student? Student { get; set; }
    public Course? Course { get; set; }
}
