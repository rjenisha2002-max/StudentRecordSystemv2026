using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudentRecordSystem.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireLoginAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        if (session.GetString("UserId") == null)
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
        }
        base.OnActionExecuting(context);
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireRoleAttribute : ActionFilterAttribute
{
    private readonly string _role;
    public RequireRoleAttribute(string role) => _role = role;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        var role = session.GetString("Role");
        if (role == null)
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }
        if (role != _role)
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
        }
        base.OnActionExecuting(context);
    }
}

public static class SessionHelper
{
    public static int? GetUserId(IHttpContextAccessor accessor)
    {
        var val = accessor.HttpContext?.Session.GetString("UserId");
        return val != null ? int.Parse(val) : null;
    }

    public static string? GetRole(IHttpContextAccessor accessor)
        => accessor.HttpContext?.Session.GetString("Role");

    public static string? GetUsername(IHttpContextAccessor accessor)
        => accessor.HttpContext?.Session.GetString("Username");
}
