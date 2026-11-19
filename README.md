# Semana 13 – Code First (ASP.NET Core + EF Core + SQL Server)

Web API con controladores, .NET 10 y EF Core 10.0.15 (`Microsoft.EntityFrameworkCore.SqlServer`). El esquema se genera con migraciones Code First.

## Modelo

| Tabla | Columnas |
|---|---|
| `Grade` | `idGrade` INT PK, `Name` VARCHAR(45), `Description` VARCHAR(45), `IsDeleted` BIT NOT NULL DEFAULT 0 |
| `Course` | `idCourse` INT PK, `Name` VARCHAR(45), `Credit` INT, `IsDeleted` BIT NOT NULL DEFAULT 0 |
| `Student` | `idStudent` INT PK, `Grade_idGrade` INT FK → `Grade`, `FirstName`, `LastName`, `Phone`, `Email` (VARCHAR(45)) |
| `Enrollment` | `idEnrollment` INT PK, `Student_idStudent` INT FK → `Student`, `Course_idCourse` INT FK → `Course`, `Date` DATETIME |

Las tres claves foráneas usan `ON DELETE NO ACTION` (Restrict). Supuesto: el diagrama no indica nulabilidad; `Name`, `FirstName` y `LastName` son obligatorios, y `Description`, `Phone` y `Email` aceptan `NULL`.

## Requisitos en Windows

- .NET SDK 10
- SQL Server LocalDB (viene con Visual Studio) o SQL Server Express
- `sqlcmd`, SSMS o Azure Data Studio para cargar datos

## Cadena de conexión

En `Semana13/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=Semana13Db;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

## Crear la base y ejecutar (Windows)

```powershell
git clone <url-del-repo> -b main
cd Semana13
dotnet tool restore                 # instala dotnet-ef 10.0.15 (dotnet-tools.json)
dotnet build
dotnet ef database update --project Semana13
dotnet run --project Semana13       # http://localhost:5239
```

La migración `InitialCreate` (carpeta `Semana13/Migrations`) crea las cuatro tablas, las tres claves foráneas y las columnas `IsDeleted`.

## Datos de prueba (ficticios)

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -d Semana13Db -i scripts/seed-data.sql
```

También puedes abrir `scripts/seed-data.sql` en SSMS o Azure Data Studio y ejecutarlo. El script solo carga datos si `Grade` está vacía. Crea:

- Grados: Primero (1), Segundo (2).
- Cursos: Matematica, Programacion, Fisica, Historia y `Matematica Avanzada` (eliminado lógicamente).
- **Ana Torres** (Primero): dos matrículas activas (Matematica, Programacion) y una en el curso eliminado.
- **Luis Anaya** (Primero): una matrícula (Fisica).
- **Beatriz Quispe** (Primero): **sin matrículas**.
- **Carlos Ramos** (Segundo): una matrícula (Historia).

## Endpoints

Las pruebas están en [`Semana13/Semana13.http`](Semana13/Semana13.http).

### CRUD (eliminación lógica)

| Método | Ruta | Respuesta |
|---|---|---|
| GET | `/api/grades`, `/api/courses` | 200, solo `IsDeleted = 0`, ordenados por nombre |
| GET | `/api/grades/{id}`, `/api/courses/{id}` | 200 / 404 si no existe o está eliminado |
| POST | `/api/grades`, `/api/courses` | 201 |
| PUT | `/api/grades/{id}`, `/api/courses/{id}` | 204 / 404 si no existe o está eliminado |
| DELETE | `/api/grades/{id}`, `/api/courses/{id}` | 204 y `IsDeleted = 1`; la fila y sus relaciones se conservan. 404 si ya estaba eliminado |

### Búsquedas

| Ruta | Regla |
|---|---|
| `GET /api/students?search=ana` | Coincidencia parcial en `FirstName`, `LastName` o `Email` (sin `search` devuelve todos). Orden: apellido descendente. |
| `GET /api/students/by-grade?name=ana&gradeId=1` | `gradeId` obligatorio; `name` filtra parcialmente `FirstName`. Solo grados activos. Devuelve el estudiante, su grado y `orderingCourseName`. **Una fila por estudiante**, ordenada por el nombre de curso **activo** más alto alfabéticamente (el primero de Z a A, es decir `MAX`) en orden descendente. Los estudiantes sin curso activo van **al final** (desempate: apellido y luego id). |
| `GET /api/enrollments/by-course?courseName=mat` | `courseName` obligatorio (400 si falta); coincidencia parcial en el nombre del curso; solo cursos activos. Una fila por matrícula con estudiante, curso y matrícula. Orden: curso ASC, apellido ASC. |
| `GET /api/enrollments/by-grade?gradeId=1` | Filtra por `Enrollment.Student.GradeId`; solo grados y cursos activos. Misma respuesta y orden que la anterior. |

Resultados esperados con los datos de prueba (`gradeId=1`): `students/by-grade` → Ana (Programacion), Luis (Fisica), Beatriz (sin curso, al final); `enrollments/by-grade` → Luis/Fisica, Ana/Matematica, Ana/Programacion.

## Migraciones (solo si cambia el modelo)

```powershell
dotnet ef migrations add NombreDeLaMigracion --project Semana13
dotnet ef database update --project Semana13
```
