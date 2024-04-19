using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager_ASP.Net_Core.Filters.ResorceFilters
{
    public class FeatureDisableResourceFilter : IAsyncResourceFilter
    {
        private readonly bool _disable;

        public FeatureDisableResourceFilter(bool disable=true)
        {
            _disable = disable;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if(_disable)
            {
                context.Result = new NotFoundResult();
            }
            else
            {
                await next();
            }
        }
    }
}
