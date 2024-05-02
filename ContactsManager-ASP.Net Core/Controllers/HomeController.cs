using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager_ASP.Net_Core.Controllers
{
	public class HomeController : Controller
	{
		[Route("[Action]")]
		public IActionResult Error()
		{
			IExceptionHandlerPathFeature? exceptionHandlerFeature= HttpContext.Features.Get<IExceptionHandlerPathFeature>();
			if(exceptionHandlerFeature != null&&exceptionHandlerFeature.Error!=null)
			{
				ViewBag.ErrorMessage = exceptionHandlerFeature.Error.Message;
			} // To catch the exception message raised during request pipeline
			return View();
		}
	}
}
