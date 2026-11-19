-- Datos ficticios para Semana13Db. Ejecutar después de `dotnet ef database update`.
-- Con la base recién creada los IDs identity empiezan en 1 (Primero = 1, Segundo = 2).
USE Semana13Db;
GO

IF EXISTS (SELECT 1 FROM Grade)
BEGIN
    RAISERROR('La base ya contiene datos; no se vuelve a cargar.', 16, 1);
    SET NOEXEC ON;
END
GO

INSERT INTO Grade (Name, Description) VALUES ('Primero', 'Primer grado'), ('Segundo', 'Segundo grado');

INSERT INTO Course (Name, Credit, IsDeleted) VALUES
    ('Matematica', 4, 0),
    ('Programacion', 5, 0),
    ('Fisica', 3, 0),
    ('Historia', 2, 0),
    ('Matematica Avanzada', 4, 1);   -- eliminado lógicamente

DECLARE @g1 INT = (SELECT idGrade FROM Grade WHERE Name = 'Primero');
DECLARE @g2 INT = (SELECT idGrade FROM Grade WHERE Name = 'Segundo');

INSERT INTO Student (Grade_idGrade, FirstName, LastName, Phone, Email) VALUES
    (@g1, 'Ana',      'Torres', '999111222', 'ana.torres@example.com'),
    (@g1, 'Luis',     'Anaya',  '999333444', 'luis.anaya@example.com'),
    (@g1, 'Beatriz',  'Quispe', '999555666', 'beatriz.quispe@example.com'),  -- sin matrículas
    (@g2, 'Carlos',   'Ramos',  '999777888', 'carlos.ramos@example.com');

DECLARE @ana     INT = (SELECT idStudent FROM Student WHERE Email = 'ana.torres@example.com');
DECLARE @luis    INT = (SELECT idStudent FROM Student WHERE Email = 'luis.anaya@example.com');
DECLARE @carlos  INT = (SELECT idStudent FROM Student WHERE Email = 'carlos.ramos@example.com');

DECLARE @mat     INT = (SELECT idCourse FROM Course WHERE Name = 'Matematica');
DECLARE @prog    INT = (SELECT idCourse FROM Course WHERE Name = 'Programacion');
DECLARE @fis     INT = (SELECT idCourse FROM Course WHERE Name = 'Fisica');
DECLARE @hist    INT = (SELECT idCourse FROM Course WHERE Name = 'Historia');
DECLARE @matAdv  INT = (SELECT idCourse FROM Course WHERE Name = 'Matematica Avanzada');

INSERT INTO Enrollment (Student_idStudent, Course_idCourse, [Date]) VALUES
    (@ana,    @mat,    '2026-03-01'),   -- Ana: dos matrículas activas
    (@ana,    @prog,   '2026-03-02'),
    (@ana,    @matAdv, '2026-03-03'),   -- curso eliminado: se ignora en las búsquedas
    (@luis,   @fis,    '2026-03-04'),
    (@carlos, @hist,   '2026-03-05');
GO

SET NOEXEC OFF;
GO
