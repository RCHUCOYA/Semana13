using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Semana13.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    idCourse = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(45)", nullable: false),
                    Credit = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.idCourse);
                });

            migrationBuilder.CreateTable(
                name: "Grade",
                columns: table => new
                {
                    idGrade = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(45)", nullable: false),
                    Description = table.Column<string>(type: "varchar(45)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grade", x => x.idGrade);
                });

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    idStudent = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Grade_idGrade = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(45)", nullable: false),
                    LastName = table.Column<string>(type: "varchar(45)", nullable: false),
                    Phone = table.Column<string>(type: "varchar(45)", nullable: true),
                    Email = table.Column<string>(type: "varchar(45)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.idStudent);
                    table.ForeignKey(
                        name: "FK_Student_Grade_Grade_idGrade",
                        column: x => x.Grade_idGrade,
                        principalTable: "Grade",
                        principalColumn: "idGrade",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Enrollment",
                columns: table => new
                {
                    idEnrollment = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Student_idStudent = table.Column<int>(type: "int", nullable: false),
                    Course_idCourse = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollment", x => x.idEnrollment);
                    table.ForeignKey(
                        name: "FK_Enrollment_Course_Course_idCourse",
                        column: x => x.Course_idCourse,
                        principalTable: "Course",
                        principalColumn: "idCourse",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Enrollment_Student_Student_idStudent",
                        column: x => x.Student_idStudent,
                        principalTable: "Student",
                        principalColumn: "idStudent",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_Course_idCourse",
                table: "Enrollment",
                column: "Course_idCourse");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_Student_idStudent",
                table: "Enrollment",
                column: "Student_idStudent");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Grade_idGrade",
                table: "Student",
                column: "Grade_idGrade");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Enrollment");

            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropTable(
                name: "Grade");
        }
    }
}
