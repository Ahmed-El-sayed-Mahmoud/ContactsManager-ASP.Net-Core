using Entities;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.ValidationHelpers;

namespace Services
{
    public class PersonServices : IPersonServices
    {
        private readonly ICountryServices _countriesService;
        private readonly PersonsDbContext _db;
        public PersonServices(PersonsDbContext personsDbContext , ICountryServices countryServices)
        {
            _countriesService =countryServices;
            _db = personsDbContext;
        }

        public async Task<PersonResponse> AddPerson(AddPersonRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            ValidationHelper.ModelValidator(request);
            int duplicate = await _db.Persons.CountAsync(temp => (temp.PersonName == request.PersonName!.Trim())&&(temp.Email == request.Email!.Trim()) );

            if (duplicate>0)
            {
                throw new ArgumentException("This Contact already exist with the same name and Email");
            }
            Person person = request.ToPerson();
            person.Country =( await _countriesService.GetCountryById(request.CountryId))?.CountryName;
            //_db.Persons.Add(person);
            //_db.SaveChanges();
            _db.sp_AddPerson(person);
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = (await _countriesService.GetCountryById(personResponse.CountryID))?.CountryName;
            return personResponse;

        }

        public async Task<bool> DeletePerson(Guid? ID)
        {
            if (ID == null) throw new ArgumentNullException();
            if (ID == Guid.Empty) throw new ArgumentNullException(nameof(ID));
            Person? person =await  _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == ID);
            if (person == null)
            {
                return false;
            }
            _db.Persons.Remove(person);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<PersonResponse>> GetAllPeople()
        {
            // return _db.sp_GetAllPersons().Select(temp=>temp.ToPersonResponse()).ToList();
            var persons = await _db.Persons.Include("country").ToListAsync();
           return persons.Select(t=>t.ToPersonResponse()).ToList();
        }
        public async Task<List<PersonResponse>> GetFiltered(string? SearchString, string? SearchBy)
        {
            SearchString = SearchString?.Trim();
            List<PersonResponse> All = await GetAllPeople();
            List<PersonResponse> Filtered =await  GetAllPeople();
            switch (SearchBy)
            {
                case nameof(PersonResponse.PersonName):
                    Filtered = All.Where((temp) => temp.PersonName != null &&
                    temp.PersonName.Contains(SearchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case nameof(PersonResponse.Email):
                    Filtered = All.Where((temp) => temp.Email != null &&
                   temp.Email.Contains(SearchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case nameof(PersonResponse.Country):
                    Filtered = All.Where((temp) => temp.Country != null &&
                   temp.Country.Contains(SearchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case nameof(PersonResponse.Address):
                    Filtered = All.Where((temp) => temp.Address != null &&
                   temp.Address.Contains(SearchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case nameof(PersonResponse.Gender):
                    Filtered = All.Where((temp) => temp.Gender?.ToString() != null &&
                   temp.Gender.ToString().ToLower() == SearchString.ToLower()).ToList();
                    break;
                case nameof(PersonResponse.DateOfBirth):
                    Filtered = All.Where((temp) => temp.DateOfBirth != null &&
                   temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(SearchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                default: Filtered = All; break;
            }
            return Filtered;
        }

        public async Task<PersonResponse?> GetPersonById(Guid? ID)
        {
            if (ID == null || ID == Guid.Empty) throw new ArgumentNullException("Id is Null");
            return (await _db.Persons.FirstOrDefaultAsync((temp) => temp.PersonID == ID))?.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
                return allPersons;

            List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Gender.ToString(), StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Gender.ToString(), StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),

                _ => allPersons
            };

            return sortedPersons;
        }

        public async Task<PersonResponse> UpdatePerson(UpdatePersonRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(personUpdateRequest));
            ValidationHelper.ModelValidator(personUpdateRequest);
            Person? person = await _db.Persons.Where(temp => temp.PersonID == personUpdateRequest.PersonId).FirstOrDefaultAsync();
            if (person == null)
                throw new ArgumentException("This ID does not exist in your Contacts");
            person.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            person.Gender = personUpdateRequest.Gender;
            person.Email = personUpdateRequest.Email;
            person.DateOfBirth = personUpdateRequest.DateOfBirth;
            person.Address = personUpdateRequest.Address;
            person.CountryID = personUpdateRequest.CountryId;

            return person.ToPersonResponse();

        }
    }
}
