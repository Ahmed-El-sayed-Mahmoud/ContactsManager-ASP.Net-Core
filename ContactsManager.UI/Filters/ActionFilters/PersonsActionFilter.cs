using ContactsManager.Controllers;
using Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;
using System.Globalization;
using static OfficeOpenXml.ExcelErrorValue;

namespace ContactsManager_ASP.Net_Core.Filters.ActionFilters
{
    public class PersonsActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsActionFilter>? _logger;

        public PersonsActionFilter(ILogger<PersonsActionFilter>? logger)
        {
            _logger = logger;
           
        }

      

        public void OnActionExecuted(ActionExecutedContext context)
        {
            context.HttpContext.Response.Headers["asasas"] = "aboelseed";
            _logger.LogInformation("{FilterName} is reached", nameof(OnActionExecuted));
            Dictionary<string, object?>? ActionArgs =(Dictionary<string, object?>?) context.HttpContext.Items["Args"];
            ContactsController controller = (ContactsController)context.Controller;
            controller.ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryID), "Country" },
                { nameof(PersonResponse.Address), "Address" }
            };
            
            if(ActionArgs.ContainsKey("searchText"))
                controller.ViewBag.CurrentSearchText = Convert.ToString(ActionArgs["searchText"]);
            if (ActionArgs.ContainsKey("searchBy"))
                controller.ViewBag.CurrentSearchBy = Convert.ToString(ActionArgs["searchBy"]);
            if (ActionArgs.ContainsKey("SortBy"))
                controller.ViewBag.SortBy = Convert.ToString(ActionArgs["SortBy"]);
            if (ActionArgs.ContainsKey("sortOrderOptions"))
                controller.ViewBag.SortOrderOptions = Convert.ToString(ActionArgs["sortOrderOptions"]);
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items["Args"] = context.ActionArguments;
            if(!context.ActionArguments.ContainsKey("searchBy"))
            {
                context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                return;
            }
            string? SearchBy = Convert.ToString( context.ActionArguments["searchBy"]);
            if(String.IsNullOrEmpty(SearchBy))
            {
                context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                return;
            }
            List<string> SearchByOptions= new List<string>()
            { 
                nameof(PersonResponse.PersonName),
                nameof(PersonResponse.PersonID),
                nameof(PersonResponse.Gender),
                nameof(PersonResponse.Age),
                nameof(PersonResponse.Address),
                nameof(PersonResponse.Country),
                nameof(PersonResponse.Email)
            };
            if(!SearchByOptions.Contains(SearchBy))
            {
                context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
            }
        }
    }
}
