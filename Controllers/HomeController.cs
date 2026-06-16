using Microsoft.AspNetCore.Mvc;

namespace StudentRecordSystem.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var role = HttpContext.Session.GetString("Role");
        if (role == "Invigilator") return RedirectToAction("Dashboard", "Invigilator");
        if (role == "Student") return RedirectToAction("MyRecord", "Student");
        return RedirectToAction("Login", "Account");
    }
}
