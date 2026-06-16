# Student Record Management System

A full-stack ASP.NET Core MVC application for managing student academic records with role-based access control for Invigilators and Students.

---

## Features

### Invigilator (Admin)
- **Dashboard** with key stats: total students, class average, top performer
- **Create** student records (auto-generated 5-digit roll number)
- **View All** students in a sortable table
- **Search** by roll number
- **View Details** with visual subject-wise performance bars
- **Edit Marks** — full-page form or **inline AJAX quick-edit modal**
- **Delete** (soft delete — record is disabled, not destroyed)
- Optionally create a **student login account** when adding a student

### Student
- Secure login with session-based authentication
- **View own academic record** only — name, roll number, subject marks, total, average, grade, pass/fail status

### Technical Highlights
- ASP.NET Core 8 MVC
- ADO.NET with Stored Procedures for all data access
- BCrypt password hashing
- Session-based authentication with role filters
- AJAX mark update (no page reload)
- Client-side + server-side validation
- Responsive design

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server 2019+ (or SQL Server Express / LocalDB)

---

## Setup Instructions

### 1. Database Setup

Open SQL Server Management Studio (SSMS) or `sqlcmd` and run the scripts in order:

```sql
-- Step 1: Create database, tables, stored procedures
Database/setup.sql

-- Step 2 (optional): If auto-seeding fails, run manually
Database/seed.sql
```

The application also **auto-seeds** the admin user and sample students on first startup if the database is reachable.

### 2. Configure Connection String

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=StudentRecordDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Common values for `Server`:
- Local default instance: `localhost` or `.`
- Named instance: `localhost\SQLEXPRESS`
- LocalDB: `(localdb)\\MSSQLLocalDB`

### 3. Run the Application

```bash
cd StudentRecordSystem
dotnet restore
dotnet run
```

The app will start on `https://localhost:5001` (or the port shown in the console).

---

## Default Credentials

| Role        | Username | Password  |
|-------------|----------|-----------|
| Invigilator | `admin`  | `Admin@123` |

Sample student accounts are not created by default. To create a student login:
1. Log in as the invigilator
2. Add a student and fill in the optional **Student Username / Password** fields

---

## Project Structure

```
StudentRecordSystem/
├── Controllers/
│   ├── AccountController.cs     # Login / Logout
│   ├── HomeController.cs        # Root redirect
│   ├── InvigilatorController.cs # Full CRUD + AJAX
│   └── StudentController.cs     # View own record
├── Data/
│   ├── StudentRepository.cs     # ADO.NET + Stored Procedures
│   └── DbSeeder.cs              # Auto-seed on startup
├── Database/
│   ├── setup.sql                # Tables + Stored Procedures
│   └── seed.sql                 # Sample data
├── Filters/
│   └── AuthFilters.cs           # RequireLogin / RequireRole filters
├── Models/
│   └── Models.cs                # Student, User models
├── ViewModels/
│   └── ViewModels.cs            # Login, Create, Edit ViewModels
├── Views/
│   ├── Account/                 # Login, AccessDenied
│   ├── Invigilator/             # Dashboard, Create, Edit, Details
│   ├── Student/                 # MyRecord, NoRecord
│   └── Shared/_Layout.cshtml
└── wwwroot/
    ├── css/site.css
    └── js/site.js
```

---

## Stored Procedures

| Procedure             | Purpose                              |
|-----------------------|--------------------------------------|
| `sp_GetAllStudents`   | List all active students             |
| `sp_GetStudentByRoll` | Fetch one student by roll number     |
| `sp_GetStudentByUserId` | Fetch student linked to a user ID  |
| `sp_CreateStudent`    | Insert student, return roll number   |
| `sp_UpdateStudentMarks` | Update marks for a student         |
| `sp_DeleteStudent`    | Soft-delete (set IsActive = 0)       |
| `sp_GetUserByUsername`| Fetch user for authentication        |
| `sp_CreateUser`       | Register a new user                  |

---

## Grading Scheme

| Average Score | Grade |
|---------------|-------|
| 90 – 100      | A+    |
| 80 – 89       | A     |
| 70 – 79       | B     |
| 60 – 69       | C     |
| 50 – 59       | D     |
| Below 50      | F     |
