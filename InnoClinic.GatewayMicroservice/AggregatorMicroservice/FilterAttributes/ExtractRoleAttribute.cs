using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace AggregatorMicroservice.FilterAttributes;

public class ExtractRoleAttribute : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var action = context.RouteData.Values["action"];
        var controller = context.RouteData.Values["controller"];
        var roleClaim = context.HttpContext.User.Claims
            .Where(c => c.Type.Equals(ClaimTypes.Role))
            .FirstOrDefault();
        if (roleClaim is null)
            throw new InvalidOperationException("there is no any role claim");
        context.ActionArguments.Add("roleName", roleClaim.Value);
    }
}
