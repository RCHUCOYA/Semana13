using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Semana13.Data;
using Semana13.DTOs;
using Semana13.Models;

namespace Semana13.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CourseDto>>> GetAll(CancellationToken ct) =>
        await db.Courses.AsNoTracking()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Name)
            .Select(c => new CourseDto(c.CourseId, c.Name, c.Credit))
            .ToListAsync(ct);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CourseDto>> GetById(int id, CancellationToken ct)
    {
        var course = await db.Courses.AsNoTracking()
            .Where(c => c.CourseId == id && !c.IsDeleted)
            .Select(c => new CourseDto(c.CourseId, c.Name, c.Credit))
            .FirstOrDefaultAsync(ct);
        return course is null ? NotFound() : course;
    }

    [HttpPost]
    public async Task<ActionResult<CourseDto>> Create(CourseRequest request, CancellationToken ct)
    {
        var course = new Course { Name = request.Name, Credit = request.Credit };
        db.Courses.Add(course);
        await db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = course.CourseId },
            new CourseDto(course.CourseId, course.Name, course.Credit));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CourseRequest request, CancellationToken ct)
    {
        var course = await db.Courses.FirstOrDefaultAsync(c => c.CourseId == id && !c.IsDeleted, ct);
        if (course is null) return NotFound();

        course.Name = request.Name;
        course.Credit = request.Credit;
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var course = await db.Courses.FirstOrDefaultAsync(c => c.CourseId == id && !c.IsDeleted, ct);
        if (course is null) return NotFound();

        course.IsDeleted = true;
        await db.SaveChangesAsync(ct);
        return NoContent();
    }
}
