using ContactsManager.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts;
using ServiceContracts.DTO;

namespace ContactsManager_ASP.Net_Core.Filters.ActionFilters
{
    public class PersonsAddEditActionFilter : IAsyncActionFilter
    {
        private readonly ICountryServices _countriesService;

        public PersonsAddEditActionFilter(ICountryServices countriesService)
        {
            _countriesService = countriesService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(context.Controller is ContactsController contactsController)
            {
                if (!contactsController.ModelState.IsValid)
                {
                    List<CountryResponse> countries = await _countriesService.GetAllCountries();
                    contactsController.ViewBag.Countries = countries;
                    contactsController.ViewBag.Errors = contactsController.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    //return View(request);
                    context.Result = contactsController.View(context.ActionArguments["request"]);
                }
                else
                    await next();
            }
            else
                await next();
        }
    }
}
