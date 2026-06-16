using Microsoft.AspNetCore.Mvc;
using StudentRecordSystem.Data;
using StudentRecordSystem.Filters;
using StudentRecordSystem.ViewModels;

namespace StudentRecordSystem.Controllers;

[RequireRole("Invigilator")]
public class InvigilatorController : Controller
{
    private readonly StudentRepository _repo;

    public InvigilatorController(StudentRepository repo) => _repo = repo;

    // ── Dashboard ─────────────────────────────────────────────

    public IActionResult Dashboard()
    {
        var students = _repo.GetAllStudents();
        ViewBag.Username = HttpContext.Session.GetString("Username");
        ViewBag.TotalStudents = students.Count;
        ViewBag.AverageScore = students.Count > 0 ? students.Average(s => s.Average).ToString("F1") : "0";
        ViewBag.TopStudent = students.OrderByDescending(s => s.Total).FirstOrDefault();
        return View(students);
    }

    // ── Create ────────────────────────────────────────────────

    [HttpGet]
    public IActionResult Create() => View(new CreateStudentViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CreateStudentViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        int? userId = null;
        if (!string.IsNullOrWhiteSpace(model.Username) && !string.IsNullOrWhiteSpace(model.Password))
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            var newId = _repo.CreateUser(model.Username, hash, "Student");
            if (newId == -1)
            {
                ModelState.AddModelError("Username", "Username already exists.");
                return View(model);
            }
            userId = newId;
        }

        var roll = _repo.CreateStudent(model.Name, model.Maths, model.Physics,
            model.Chemistry, model.English, model.Programming, userId);

        TempData["Success"] = $"Student created successfully! Roll Number: {roll}";
        return RedirectToAction("Dashboard");
    }

    // ── Details ───────────────────────────────────────────────

    [HttpGet]
    public IActionResult Details(string roll)
    {
        var student = _repo.GetStudentByRoll(roll);
        if (student == null)
        {
            TempData["Error"] = "Student not found.";
            return RedirectToAction("Dashboard");
        }
        return View(student);
    }

    // ── Edit Marks ────────────────────────────────────────────

    [HttpGet]
    public IActionResult Edit(string roll)
    {
        var student = _repo.GetStudentByRoll(roll);
        if (student == null)
        {
            TempData["Error"] = "Student not found.";
            return RedirectToAction("Dashboard");
        }
        var vm = new EditMarksViewModel
        {
            RollNumber  = student.RollNumber,
            Name        = student.Name,
            Maths       = student.Maths,
            Physics     = student.Physics,
            Chemistry   = student.Chemistry,
            English     = student.English,
            Programming = student.Programming
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(EditMarksViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var ok = _repo.UpdateMarks(model.RollNumber, model.Maths, model.Physics,
            model.Chemistry, model.English, model.Programming);

        if (!ok)
        {
            TempData["Error"] = "Update failed. Student not found.";
            return RedirectToAction("Dashboard");
        }

        TempData["Success"] = $"Marks updated successfully for Roll No: {model.RollNumber}";
        return RedirectToAction("Dashboard");
    }

    // ── AJAX Edit Marks ───────────────────────────────────────

    [HttpPost]
    public IActionResult UpdateMarksAjax([FromBody] EditMarksViewModel model)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, message = "Validation failed.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

        var ok = _repo.UpdateMarks(model.RollNumber, model.Maths, model.Physics,
            model.Chemistry, model.English, model.Programming);

        if (!ok)
            return Json(new { success = false, message = "Student not found." });

        var student = _repo.GetStudentByRoll(model.RollNumber);
        return Json(new
        {
            success  = true,
            message  = "Marks updated successfully!",
            total    = student?.Total,
            average  = student?.Average.ToString("F1"),
            grade    = student?.Grade
        });
    }

    // ── Delete ────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(string roll)
    {
        var ok = _repo.DeleteStudent(roll);
        TempData[ok ? "Success" : "Error"] = ok
            ? $"Student with Roll No {roll} has been removed."
            : "Student not found.";
        return RedirectToAction("Dashboard");
    }

    // ── Search ────────────────────────────────────────────────

    [HttpGet]
    public IActionResult Search(string roll)
    {
        if (string.IsNullOrWhiteSpace(roll))
        {
            TempData["Error"] = "Please enter a roll number.";
            return RedirectToAction("Dashboard");
        }
        var student = _repo.GetStudentByRoll(roll.Trim());
        if (student == null)
        {
            TempData["Error"] = $"No student found with Roll No: {roll}";
            return RedirectToAction("Dashboard");
        }
        return View("Details", student);
    }
}
