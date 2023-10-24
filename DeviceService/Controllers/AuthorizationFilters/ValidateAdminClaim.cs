using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidateAdminClaimAttribute : TypeFilterAttribute
{
    public ValidateAdminClaimAttribute() : base(typeof(ValidateAdminClaimAttribute))
    {
    }

    private class ValidateAdminClaimFilter : IAuthorizationFilter
    {
        private readonly ILogger<ValidateAdminClaimFilter> _logger;

        public ValidateAdminClaimFilter(ILogger<ValidateAdminClaimFilter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userRoleClaim = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            
            if(userRoleClaim == "Admin")
            {
                return;
            }
            
            context.Result = new ForbidResult();
        }
    }
}
