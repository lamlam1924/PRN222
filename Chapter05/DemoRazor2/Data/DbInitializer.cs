using DemoRazor2.Models;

namespace DemoRazor2.Data;

public static class DbInitializer
{
    public static void Initialize(SchoolContext context)
    {
        if (context.Students.Any()) return;
        context.Students.AddRange(
            new Student { FirstMidName = "Carson", LastName = "Alexander", EnrollmentDate = DateTime.Parse("2019-09-01") },
            new Student { FirstMidName = "Meredith", LastName = "Alonso", EnrollmentDate = DateTime.Parse("2017-09-01") },
            new Student { FirstMidName = "Arturo", LastName = "Anand", EnrollmentDate = DateTime.Parse("2018-09-01") }
        );
        context.SaveChanges();
    }
}
