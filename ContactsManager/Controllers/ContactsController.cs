using Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
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
        public async Task<IActionResult> Index(string searchBy, string searchText, string SortBy = nameof(PersonResponse.PersonName)
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
            List<PersonResponse> persons = await _personServices.GetFiltered(searchText, searchBy);
            List<PersonResponse> SortedList = await _personServices.GetSortedPersons(persons, SortBy,
                (SortOrderOptions)Enum.Parse<SortOrderOptions>(sortOrderOptions));
            return View(SortedList);
        }
        [HttpGet]
        [Route("~/Create")]
        public async Task<IActionResult> CreateContact()
        {
            //_personServices.AddPerson(request);
            List<CountryResponse> countries = await _countryServices.GetAllCountries();
            ViewBag.Countries = countries;
            return View();
        }

        [Route("~/Create")]
        [HttpPost]
        public async Task<IActionResult> CreateContact(AddPersonRequest request)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = await _countryServices.GetAllCountries();
                ViewBag.Countries = countries;
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return View(request);
            }
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
    }
}
