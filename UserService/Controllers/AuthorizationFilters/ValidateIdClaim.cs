using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidateIdClaimAttribute : TypeFilterAttribute
{
    public ValidateIdClaimAttribute() : base(typeof(ValidateIdClaimFilter))
    {
    }

    private class ValidateIdClaimFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var routeId = context.RouteData.Values["id"] as string;
            var userRoleClaim = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            if(userRoleClaim == "Admin")
            {
                return;
            }
            if (userIdClaim == null || userIdClaim != routeId)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
