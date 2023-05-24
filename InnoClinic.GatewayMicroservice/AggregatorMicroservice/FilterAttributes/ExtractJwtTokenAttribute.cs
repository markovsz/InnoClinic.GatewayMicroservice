using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace AggregatorMicroservice.FilterAttributes;

public class ExtractJwtTokenAttribute : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var action = context.RouteData.Values["action"];
        var controller = context.RouteData.Values["controller"];
        string jwtToken = null;
        var roleClaim = context.HttpContext.Request.Headers.TryGetValue("Authorization", out var jwtTokenHeader);
        jwtToken = jwtTokenHeader.ToString().Split(' ')[1];
        if(jwtToken is null)
            throw new UnauthorizedAccessException("you aren't authorized");
        context.ActionArguments.Add("authParam", jwtToken);
    }
}
