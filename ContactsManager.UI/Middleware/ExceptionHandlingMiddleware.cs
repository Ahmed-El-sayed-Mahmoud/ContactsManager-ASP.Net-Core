using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ContactsManager_ASP.Net_Core.Middleware
{
	// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionHandlingMiddleware> _logger;
		public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task Invoke(HttpContext httpContext)
		{

			try
			{
				await _next(httpContext);
			}
			catch (Exception e)
			{
				if(e.InnerException!=null)
				{
					_logger.LogError("{ExceptionType} occured : {ExceptionMessage}",e.InnerException.GetType(),
						e.InnerException.Message);
				}
				else
				{
					_logger.LogError("{ExceptionType} occured : {ExceptionMessage}", e.GetType(),
						e.Message);
				}
				//httpContext.Response.StatusCode = 500;
				//await httpContext.Response.WriteAsync("Error occurred");
				throw;
			}
		}
	}

	// Extension method used to add the middleware to the HTTP request pipeline.
	public static class MiddlewareExtensions
	{
		public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<ExceptionHandlingMiddleware>();
		}
	}
}
