using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager_ASP.Net_Core.Filters.ActionFilters
{

    public class ResponseHeadersActionFilter : ActionFilterAttribute /*IAsyncActionFilter,IOrderedFilter*/
    {
        private readonly string? _key;
        private readonly string? _value;
        public int Order { get; set; }
        public ResponseHeadersActionFilter( string? key, string? value, int order)
        {
            _key = key;
            _value = value;
            Order = order;  
        }

        //public void OnActionExecuted(ActionExecutedContext context)
        //{
        //    _logger?.LogInformation("{ActionName}.{ActionMethod} is reached", nameof(ResponseHeadersActionFilter)
        //        , nameof(OnActionExecuted));

        //}

        //public void OnActionExecuting(ActionExecutingContext context)
        //{
        //    context.HttpContext.Response.Headers[_key]=_value;
        //}

        public  override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            
            context.HttpContext.Response.Headers[_key] = _value; // Logic like  OnActionExecuting
            await next();
            //logic like  OnActionExecuted
           
            //context.HttpContext.Response.Headers[_key] = _value+"aboelseed";
        }
    }
}
