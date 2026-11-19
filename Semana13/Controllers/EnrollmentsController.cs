using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Semana13.Data;
using Semana13.DTOs;
using Semana13.Models;

namespace Semana13.Controllers;

[ApiController]
[Route("api/enrollments")]
public class EnrollmentsController(AppDbContext db) : ControllerBase
{
    // GET /api/enrollments/by-course?courseName=mat
    [HttpGet("by-course")]
    public async Task<ActionResult<List<EnrollmentDetailDto>>> ByCourse(
        [FromQuery] string courseName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(courseName))
            return BadRequest("courseName es obligatorio.");

        var text = courseName.Trim();
        return await ToDetails(
            db.Enrollments.AsNoTracking()
                .Where(e => !e.Course!.IsDeleted && e.Course.Name.Contains(text)), ct);
    }

    // GET /api/enrollments/by-grade?gradeId=1
    [HttpGet("by-grade")]
    public async Task<ActionResult<List<EnrollmentDetailDto>>> ByGrade(
        [FromQuery] int gradeId, CancellationToken ct) =>
        await ToDetails(
            db.Enrollments.AsNoTracking()
                .Where(e => e.Student!.GradeId == gradeId
                            && !e.Student.Grade!.IsDeleted
                            && !e.Course!.IsDeleted), ct);

    private static Task<List<EnrollmentDetailDto>> ToDetails(IQueryable<Enrollment> query, CancellationToken ct) =>
        query
            .OrderBy(e => e.Course!.Name)
            .ThenBy(e => e.Student!.LastName)
            .ThenBy(e => e.EnrollmentId)
            .Select(e => new EnrollmentDetailDto(
                e.EnrollmentId,
                e.Date,
                new StudentDto(e.Student!.StudentId, e.Student.GradeId, e.Student.FirstName,
                    e.Student.LastName, e.Student.Phone, e.Student.Email),
                new CourseDto(e.Course!.CourseId, e.Course.Name, e.Course.Credit)))
            .ToListAsync(ct);
}
