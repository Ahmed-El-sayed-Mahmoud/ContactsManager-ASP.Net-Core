using ContactsManager_ASP.Net_Core.Filters;
using ContactsManager_ASP.Net_Core.Filters.ActionFilters;
using ContactsManager_ASP.Net_Core.Filters.AuthorizationFilters;
using ContactsManager_ASP.Net_Core.Filters.ExceptionFilters;
using ContactsManager_ASP.Net_Core.Filters.ResultFilters;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IPersonGetterServices _personGetterServices;
		private readonly IPersonUpdaterServices _personUpdaterServices;
		private readonly IPersonDeleterServices _personDeleterServices;
		private readonly IPersonAdderServices _personAdderServices;
		private readonly IPersonSorterServices _personSorterServices;
		private readonly IPersonUploaderServices _personUploaderServices;
		private readonly ICountryServices _countryServices;
        public ContactsController(IPersonGetterServices personServices,
            IPersonAdderServices personAdderServices,IPersonDeleterServices personDeleterServices, IPersonSorterServices
             personSorterServices, IPersonUploaderServices personUploaderServices, IPersonUpdaterServices personUpdaterServices
            , ICountryServices countryServices)
        {
            _personGetterServices = personServices;
            _countryServices = countryServices;
            _personUploaderServices = personUploaderServices;
             _personUpdaterServices = personUpdaterServices;
            _personDeleterServices = personDeleterServices;
            _personAdderServices = personAdderServices;
            _personSorterServices = personSorterServices;
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
        
            List<PersonResponse> persons = await _personGetterServices.GetFiltered(searchText, searchBy);
            List<PersonResponse> SortedList = await _personSorterServices.GetSortedPersons(persons, sortBy,
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
           
            await _personAdderServices.AddPerson(request);
            return RedirectToAction("Index");
        }
        [Route("[action]")]
        public async Task<IActionResult> personsPDF()
        {
            List<PersonResponse> AllPersons = await _personGetterServices.GetAllPeople();

            return new ViewAsPdf("PersonsPDF", AllPersons, ViewData)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins() { Top = 20, Right = 20, Bottom = 20, Left = 20 },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
                
            };

        }
        [Route("[action]")]
        public async Task<IActionResult> personsExcel()
        {
            MemoryStream memoryStream = await _personGetterServices.GetPersonsExcel();
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
                ViewBag.Message= $"{ await  _personUploaderServices.UploadExcelFile(formFile)} contacts ware Added Successfully";
            }
            return View();
        }
    }
}
