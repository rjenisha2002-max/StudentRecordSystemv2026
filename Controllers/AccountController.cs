using Microsoft.AspNetCore.Mvc;
using StudentRecordSystem.Data;
using StudentRecordSystem.ViewModels;

namespace StudentRecordSystem.Controllers;

public class AccountController : Controller
{
    private readonly StudentRepository _repo;

    public AccountController(StudentRepository repo) => _repo = repo;

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (HttpContext.Session.GetString("UserId") != null)
            return RedirectBasedOnRole();

        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = _repo.GetUserByUsername(model.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError("", "Invalid username or password.");
            return View(model);
        }

        HttpContext.Session.SetString("UserId", user.UserId.ToString());
        HttpContext.Session.SetString("Username", user.Username);
        HttpContext.Session.SetString("Role", user.Role);

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectBasedOnRole();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied() => View();

    private IActionResult RedirectBasedOnRole()
    {
        var role = HttpContext.Session.GetString("Role");
        return role == "Invigilator"
            ? RedirectToAction("Dashboard", "Invigilator")
            : RedirectToAction("MyRecord", "Student");
    }
}
