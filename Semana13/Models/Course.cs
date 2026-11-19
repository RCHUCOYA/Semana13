namespace Semana13.Models;

public class Course
{
    public int CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Credit { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
