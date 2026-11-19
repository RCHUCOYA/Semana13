using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Semana13.Data;
using Semana13.DTOs;

namespace Semana13.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController(AppDbContext db) : ControllerBase
{
    // GET /api/students?search=ana
    [HttpGet]
    public async Task<ActionResult<List<StudentDto>>> Search([FromQuery] string? search, CancellationToken ct)
    {
        var query = db.Students.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var text = search.Trim();
            query = query.Where(s =>
                s.FirstName.Contains(text) ||
                s.LastName.Contains(text) ||
                (s.Email != null && s.Email.Contains(text)));
        }

        return await query
            .OrderByDescending(s => s.LastName)
            .ThenBy(s => s.StudentId)
            .Select(s => new StudentDto(s.StudentId, s.GradeId, s.FirstName, s.LastName, s.Phone, s.Email))
            .ToListAsync(ct);
    }

    // GET /api/students/by-grade?name=ana&gradeId=1
    [HttpGet("by-grade")]
    public async Task<ActionResult<List<StudentWithGradeDto>>> ByGrade(
        [FromQuery] int gradeId, [FromQuery] string? name, CancellationToken ct)
    {
        var query = db.Students.AsNoTracking()
            .Where(s => s.GradeId == gradeId && !s.Grade!.IsDeleted);

        if (!string.IsNullOrWhiteSpace(name))
        {
            var text = name.Trim();
            query = query.Where(s => s.FirstName.Contains(text));
        }

        // Un estudiante = una fila; se ordena por el mayor nombre de curso activo (Z-A).
        return await query
            .Select(s => new
            {
                Student = s,
                TopCourse = s.Enrollments
                    .Where(e => !e.Course!.IsDeleted)
                    .Select(e => e.Course!.Name)
                    .Max()
            })
            .OrderBy(x => x.TopCourse == null)
            .ThenByDescending(x => x.TopCourse)
            .ThenBy(x => x.Student.LastName)
            .ThenBy(x => x.Student.StudentId)
            .Select(x => new StudentWithGradeDto(
                new StudentDto(x.Student.StudentId, x.Student.GradeId, x.Student.FirstName,
                    x.Student.LastName, x.Student.Phone, x.Student.Email),
                new GradeDto(x.Student.Grade!.GradeId, x.Student.Grade.Name, x.Student.Grade.Description),
                x.TopCourse))
            .ToListAsync(ct);
    }
}
