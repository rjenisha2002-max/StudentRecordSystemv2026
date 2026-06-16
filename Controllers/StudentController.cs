using Microsoft.AspNetCore.Mvc;
using StudentRecordSystem.Data;
using StudentRecordSystem.Filters;

namespace StudentRecordSystem.Controllers;

[RequireRole("Student")]
public class StudentController : Controller
{
    private readonly StudentRepository _repo;

    public StudentController(StudentRepository repo) => _repo = repo;

    public IActionResult MyRecord()
    {
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (userIdStr == null) return RedirectToAction("Login", "Account");

        var userId = int.Parse(userIdStr);
        var student = _repo.GetStudentByUserId(userId);

        if (student == null)
        {
            ViewBag.Error = "No record found for your account. Please contact your invigilator.";
            return View("NoRecord");
        }

        ViewBag.Username = HttpContext.Session.GetString("Username");
        return View(student);
    }
}
