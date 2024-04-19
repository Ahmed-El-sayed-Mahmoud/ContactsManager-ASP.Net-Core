using ContactsManager_ASP.Net_Core.Filters;
using ContactsManager_ASP.Net_Core.Filters.ActionFilters;
using ContactsManager_ASP.Net_Core.Filters.AuthorizationFilters;
using ContactsManager_ASP.Net_Core.Filters.ExceptionFilters;
using ContactsManager_ASP.Net_Core.Filters.ResultFilters;
using Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
namespace ContactsManager.Controllers
{
    [TypeFilter(typeof(HandlingExceptionFilter))]
    [TypeFilter(typeof(TokenAuth))]
    public class ContactsController : Controller
    {
        private readonly IPersonServices _personServices;
        private readonly ICountryServices _countryServices;
        public ContactsController(IPersonServices personServices, ICountryServices countryServices)
        {
            _personServices = personServices;
            _countryServices = countryServices;
        }
        [Route("/")]
        [Route("/Index")]
        [TypeFilter(typeof(TokenResultFilter))]
        [SkipFilter]
        [TypeFilter(typeof(PersonsActionFilter), Order =-1)]
        [ResponseHeadersActionFilter("myKey","myValue",2)]
        public async Task<IActionResult> Index(string searchBy, string searchText, string sortBy = nameof(PersonResponse.PersonName)
            , string sortOrderOptions = "ASC")
        {
        
            List<PersonResponse> persons = await _personServices.GetFiltered(searchText, searchBy);
            List<PersonResponse> SortedList = await _personServices.GetSortedPersons(persons, sortBy,
                (SortOrderOptions)Enum.Parse<SortOrderOptions>(sortOrderOptions));
            return View(SortedList);
        }
        [HttpGet]
        [Route("~/Create")]
        // [TypeFilter(typeof(FeatureDisableResourceFilter))]
      
        public async Task<IActionResult> CreateContact()
        {
            //_personServices.AddPerson(request);
            List<CountryResponse> countries = await _countryServices.GetAllCountries();
            ViewBag.Countries = countries;
            return View();
        }

        [Route("~/Create")]
        [HttpPost]
        [TypeFilter(typeof(PersonsAddEditActionFilter))]
        
        public async Task<IActionResult> CreateContact(AddPersonRequest request)
        {
           
            await _personServices.AddPerson(request);
            return RedirectToAction("Index");
        }
        [Route("[action]")]
        public async Task<IActionResult> personsPDF()
        {
            List<PersonResponse> AllPersons = await _personServices.GetAllPeople();

            return new ViewAsPdf("PersonsPDF", AllPersons, ViewData)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins() { Top = 20, Right = 20, Bottom = 20, Left = 20 },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
                
            };

        }
        [Route("[action]")]
        public async Task<IActionResult> personsExcel()
        {
            MemoryStream memoryStream = await _personServices.GetPersonsExcel();
            return File(memoryStream, "pplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet","persons.xlsx");

        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> UploadExcel()
        {
            return View();
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult>UploadExcel(IFormFile formFile)
        {
            if (formFile == null)
                ViewBag.ErrorMessage = "No File was Uploaded";
            else if(Path.GetExtension(formFile.FileName).ToLower()!=".xlsx")
            {
                ViewBag.ErrorMessage = "The file should be with extension .xlsx";
            }
            else
            {
                ViewBag.Message= $"{ await  _personServices.UploadExcelFile(formFile)} contacts ware Added Successfully";
            }
            return View();
        }
    }
}
