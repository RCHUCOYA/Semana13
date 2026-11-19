using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Semana13.Data;
using Semana13.DTOs;
using Semana13.Models;

namespace Semana13.Controllers;

[ApiController]
[Route("api/grades")]
public class GradesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<GradeDto>>> GetAll(CancellationToken ct) =>
        await db.Grades.AsNoTracking()
            .Where(g => !g.IsDeleted)
            .OrderBy(g => g.Name)
            .Select(g => new GradeDto(g.GradeId, g.Name, g.Description))
            .ToListAsync(ct);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GradeDto>> GetById(int id, CancellationToken ct)
    {
        var grade = await db.Grades.AsNoTracking()
            .Where(g => g.GradeId == id && !g.IsDeleted)
            .Select(g => new GradeDto(g.GradeId, g.Name, g.Description))
            .FirstOrDefaultAsync(ct);
        return grade is null ? NotFound() : grade;
    }

    [HttpPost]
    public async Task<ActionResult<GradeDto>> Create(GradeRequest request, CancellationToken ct)
    {
        var grade = new Grade { Name = request.Name, Description = request.Description };
        db.Grades.Add(grade);
        await db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = grade.GradeId },
            new GradeDto(grade.GradeId, grade.Name, grade.Description));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, GradeRequest request, CancellationToken ct)
    {
        var grade = await db.Grades.FirstOrDefaultAsync(g => g.GradeId == id && !g.IsDeleted, ct);
        if (grade is null) return NotFound();

        grade.Name = request.Name;
        grade.Description = request.Description;
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var grade = await db.Grades.FirstOrDefaultAsync(g => g.GradeId == id && !g.IsDeleted, ct);
        if (grade is null) return NotFound();

        grade.IsDeleted = true;
        await db.SaveChangesAsync(ct);
        return NoContent();
    }
}
