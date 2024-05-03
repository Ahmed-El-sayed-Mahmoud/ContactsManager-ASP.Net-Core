using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager_ASP.Net_Core.Filters.AuthorizationFilters
{
    public class TokenAuth : IAsyncAuthorizationFilter
    {
        public  Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.Filters.OfType<SkipFilter>().Any())
            {
                return Task.CompletedTask;
            }
            if (!context.HttpContext.Request.Cookies.ContainsKey("AuthKey")
                && context.HttpContext.Request.Cookies["AuthKey"]!="A100")
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }
            return Task.CompletedTask;
        }
    }
}
