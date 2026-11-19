using System.ComponentModel.DataAnnotations;

namespace Semana13.DTOs;

public class GradeRequest
{
    [Required, StringLength(45)]
    public string Name { get; set; } = string.Empty;

    [StringLength(45)]
    public string? Description { get; set; }
}

public record GradeDto(int GradeId, string Name, string? Description);

public class CourseRequest
{
    [Required, StringLength(45)]
    public string Name { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Credit { get; set; }
}

public record CourseDto(int CourseId, string Name, int Credit);

public record StudentDto(int StudentId, int GradeId, string FirstName, string LastName, string? Phone, string? Email);

public record StudentWithGradeDto(
    StudentDto Student,
    GradeDto Grade,
    string? OrderingCourseName);

public record EnrollmentDetailDto(int EnrollmentId, DateTime Date, StudentDto Student, CourseDto Course);
