using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager_ASP.Net_Core.Filters.ResultFilters
{
    public class TokenResultFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
            return;
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Cookies.Append("AuthKey", "A100");
        }

        
    }
}
