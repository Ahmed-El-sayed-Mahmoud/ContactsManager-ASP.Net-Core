using Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
namespace ContactsManager.Controllers
{
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
        [Route("~/Index")]
        public IActionResult Index(string searchBy, string searchText, string SortBy = nameof(PersonResponse.PersonName)
            , string sortOrderOptions = "ASC")
        {
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryID), "Country" },
                { nameof(PersonResponse.Address), "Address" }
            };
            ViewBag.CurrentSearchText = searchText;
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.SortBy = SortBy;
            ViewBag.SortOrderOptions = sortOrderOptions;
            List<PersonResponse> persons = _personServices.GetFiltered(searchText, searchBy);
            List<PersonResponse> SortedList = _personServices.GetSortedPersons(persons, SortBy,
                (SortOrderOptions)Enum.Parse<SortOrderOptions>(sortOrderOptions));
            return View(SortedList);
        }
        [HttpGet]
        [Route("~/Create")]
        public IActionResult CreateContact()
        {
            //_personServices.AddPerson(request);
            List<CountryResponse> countries = _countryServices.GetAllCountries();
            ViewBag.Countries = countries;
            return View();
        }

        [Route("~/Create")]
        [HttpPost]
        public IActionResult CreateContact(AddPersonRequest request)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = _countryServices.GetAllCountries();
                ViewBag.Countries = countries;
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return View(request);
            }
            _personServices.AddPerson(request);
            return RedirectToAction("Index");
        }

    }
}
