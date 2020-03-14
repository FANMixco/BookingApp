using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookingApp.Filters.Authorization
{
    public class AuthorizeAttribute : TypeFilterAttribute
    {
        public AuthorizeAttribute(params string[] claim) : base(typeof(AuthorizeFilter))
        {
            Arguments = new object[] { claim };
        }
    }

    public class AuthorizeFilter : IAuthorizationFilter
    {
        readonly string[] _claim;

        public AuthorizeFilter(params string[] claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                bool flagClaim = false;
                foreach (var item in _claim)
                {
                    if ((context.HttpContext.User.Identity as ClaimsIdentity).HasClaim("ROLES", item))
                    {
                        flagClaim = true;
                    }
                }
                if (!flagClaim)
                {
                    context.Result = new RedirectResult("~/Home/Error");
                }
            }
            else
            {
                context.Result = new RedirectResult($"~/{context.HttpContext.Request.Path.ToString().Remove(0, 1)}");
            }
            return;
        }
    }
}
