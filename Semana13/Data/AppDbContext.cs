using Microsoft.EntityFrameworkCore;
using Semana13.Models;

namespace Semana13.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Grade>(e =>
        {
            e.ToTable("Grade");
            e.HasKey(x => x.GradeId);
            e.Property(x => x.GradeId).HasColumnName("idGrade").HasColumnType("int").ValueGeneratedOnAdd();
            e.Property(x => x.Name).HasColumnName("Name").HasColumnType("varchar(45)").IsRequired();
            e.Property(x => x.Description).HasColumnName("Description").HasColumnType("varchar(45)");
            e.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasColumnType("bit").IsRequired().HasDefaultValue(false);
        });

        modelBuilder.Entity<Course>(e =>
        {
            e.ToTable("Course");
            e.HasKey(x => x.CourseId);
            e.Property(x => x.CourseId).HasColumnName("idCourse").HasColumnType("int").ValueGeneratedOnAdd();
            e.Property(x => x.Name).HasColumnName("Name").HasColumnType("varchar(45)").IsRequired();
            e.Property(x => x.Credit).HasColumnName("Credit").HasColumnType("int");
            e.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasColumnType("bit").IsRequired().HasDefaultValue(false);
        });

        modelBuilder.Entity<Student>(e =>
        {
            e.ToTable("Student");
            e.HasKey(x => x.StudentId);
            e.Property(x => x.StudentId).HasColumnName("idStudent").HasColumnType("int").ValueGeneratedOnAdd();
            e.Property(x => x.GradeId).HasColumnName("Grade_idGrade").HasColumnType("int").IsRequired();
            e.Property(x => x.FirstName).HasColumnName("FirstName").HasColumnType("varchar(45)").IsRequired();
            e.Property(x => x.LastName).HasColumnName("LastName").HasColumnType("varchar(45)").IsRequired();
            e.Property(x => x.Phone).HasColumnName("Phone").HasColumnType("varchar(45)");
            e.Property(x => x.Email).HasColumnName("Email").HasColumnType("varchar(45)");

            e.HasOne(x => x.Grade)
                .WithMany(g => g.Students)
                .HasForeignKey(x => x.GradeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Enrollment>(e =>
        {
            e.ToTable("Enrollment");
            e.HasKey(x => x.EnrollmentId);
            e.Property(x => x.EnrollmentId).HasColumnName("idEnrollment").HasColumnType("int").ValueGeneratedOnAdd();
            e.Property(x => x.StudentId).HasColumnName("Student_idStudent").HasColumnType("int").IsRequired();
            e.Property(x => x.CourseId).HasColumnName("Course_idCourse").HasColumnType("int").IsRequired();
            e.Property(x => x.Date).HasColumnName("Date").HasColumnType("datetime");

            e.HasOne(x => x.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
