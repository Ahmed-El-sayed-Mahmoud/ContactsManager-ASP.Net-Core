using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager_ASP.Net_Core.Filters.ExceptionFilters
{
    public class HandlingExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<HandlingExceptionFilter> _logger;
        private readonly IHostEnvironment _hostEnvironment;
        public HandlingExceptionFilter(IHostEnvironment hostEnvironment,ILogger<HandlingExceptionFilter> logger)
        {
            _hostEnvironment = hostEnvironment; // to know the current Environment
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError
                (" Exception happend caught by {FilterName} :{ExceptionType} : {ExceptionMessage}", nameof(HandlingExceptionFilter),
                 context.Exception.GetType(), context.Exception.Message);
            if(_hostEnvironment.IsDevelopment())
            {
                context.Result = new ContentResult() { Content = context.Exception.ToString() }; //short circuiting

            }
        }
    }
}
