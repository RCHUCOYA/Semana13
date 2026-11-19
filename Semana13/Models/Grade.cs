namespace Semana13.Models;

public class Grade
{
    public int GradeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<Student> Students { get; set; } = new List<Student>();
}
