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
        private readonly ILogger<ValidateIdClaimFilter> _logger;

        public ValidateIdClaimFilter(ILogger<ValidateIdClaimFilter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var routeId = context.RouteData.Values["id"] as string;
            var userRoleClaim = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            
            _logger.LogInformation($"UserIdClaim: {userIdClaim}, RouteId: {routeId}, UserRoleClaim: {userRoleClaim}");

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
