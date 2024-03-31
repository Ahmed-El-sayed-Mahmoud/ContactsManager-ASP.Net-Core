using Entities;
using Entities.Enums;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.ValidationHelpers;

namespace Services
{
    public class PersonServices : IPersonServices
    {
        private readonly List<Person> _people;
        private readonly ICountryServices _countriesService;
        public PersonServices(bool initialize = true)
        {
            _people = new List<Person>();
            _countriesService = new CountryServices();
            if (initialize)
            {
                _people.Add(new Person() { PersonID = Guid.Parse("8082ED0C-396D-4162-AD1D-29A13F929824"), PersonName = "Aguste", Email = "aleddy0@booking.com", DateOfBirth = DateTime.Parse("1993-01-02"), Gender = GenderOptions.Male, Address = "0858 Novick Terrace", ReceiveNewsLetters = false, CountryID = Guid.Parse("000C76EB-62E9-4465-96D1-2C41FDB64C3B") });

                _people.Add(new Person() { PersonID = Guid.Parse("06D15BAD-52F4-498E-B478-ACAD847ABFAA"), PersonName = "Jasmina", Email = "jsyddie1@miibeian.gov.cn", DateOfBirth = DateTime.Parse("1991-06-24"), Gender = GenderOptions.Female, Address = "0742 Fieldstone Lane", ReceiveNewsLetters = true, CountryID = Guid.Parse("32DA506B-3EBA-48A4-BD86-5F93A2E19E3F") });

                _people.Add(new Person() { PersonID = Guid.Parse("D3EA677A-0F5B-41EA-8FEF-EA2FC41900FD"), PersonName = "Kendall", Email = "khaquard2@arstechnica.com", DateOfBirth = DateTime.Parse("1993-08-13"), Gender = GenderOptions.Male, Address = "7050 Pawling Alley", ReceiveNewsLetters = false, CountryID = Guid.Parse("32DA506B-3EBA-48A4-BD86-5F93A2E19E3F") });

                _people.Add(new Person() { PersonID = Guid.Parse("89452EDB-BF8C-4283-9BA4-8259FD4A7A76"), PersonName = "Kilian", Email = "kaizikowitz3@joomla.org", DateOfBirth = DateTime.Parse("1991-06-17"), Gender = GenderOptions.Male, Address = "233 Buhler Junction", ReceiveNewsLetters = true, CountryID = Guid.Parse("DF7C89CE-3341-4246-84AE-E01AB7BA476E") });

                _people.Add(new Person() { PersonID = Guid.Parse("F5BD5979-1DC1-432C-B1F1-DB5BCCB0E56D"), PersonName = "Dulcinea", Email = "dbus4@pbs.org", DateOfBirth = DateTime.Parse("1996-09-02"), Gender = GenderOptions.Female, Address = "56 Sundown Point", ReceiveNewsLetters = false, CountryID = Guid.Parse("DF7C89CE-3341-4246-84AE-E01AB7BA476E") });

                _people.Add(new Person() { PersonID = Guid.Parse("A795E22D-FAED-42F0-B134-F3B89B8683E5"), PersonName = "Corabelle", Email = "cadams5@t-online.de", DateOfBirth = DateTime.Parse("1993-10-23"), Gender = GenderOptions.Female, Address = "4489 Hazelcrest Place", ReceiveNewsLetters = false, CountryID = Guid.Parse("15889048-AF93-412C-B8F3-22103E943A6D") });

                _people.Add(new Person() { PersonID = Guid.Parse("3C12D8E8-3C1C-4F57-B6A4-C8CAAC893D7A"), PersonName = "Faydra", Email = "fbischof6@boston.com", DateOfBirth = DateTime.Parse("1996-02-14"), Gender = GenderOptions.Female, Address = "2010 Farragut Pass", ReceiveNewsLetters = true, CountryID = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB") });

                _people.Add(new Person() { PersonID = Guid.Parse("7B75097B-BFF2-459F-8EA8-63742BBD7AFB"), PersonName = "Oby", Email = "oclutheram7@foxnews.com", DateOfBirth = DateTime.Parse("1992-05-31"), Gender = GenderOptions.Male, Address = "2 Fallview Plaza", ReceiveNewsLetters = false, CountryID = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB") });

                _people.Add(new Person() { PersonID = Guid.Parse("6717C42D-16EC-4F15-80D8-4C7413E250CB"), PersonName = "Seumas", Email = "ssimonitto8@biglobe.ne.jp", DateOfBirth = DateTime.Parse("1999-02-02"), Gender = GenderOptions.Male, Address = "76779 Norway Maple Crossing", ReceiveNewsLetters = false, CountryID = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB") });

                _people.Add(new Person() { PersonID = Guid.Parse("6E789C86-C8A6-4F18-821C-2ABDB2E95982"), PersonName = "Freemon", Email = "faugustin9@vimeo.com", DateOfBirth = DateTime.Parse("1996-04-27"), Gender = GenderOptions.Male, Address = "8754 Becker Street", ReceiveNewsLetters = false, CountryID = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB") });

            }
        }

        public PersonResponse AddPerson(AddPersonRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            ValidationHelper.ModelValidator(request);
            bool duplicate = _people.Where((temp) =>
            {
                return temp.PersonName == request.PersonName!.Trim() && temp.Email == request.Email!.Trim();
            }).Count() > 0;

            if (duplicate)
            {
                throw new ArgumentException("This Contact already exist with the same name and Email");
            }
            Person person = request.ToPerson();
            _people.Add(person);
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = _countriesService.GetCountryById(personResponse.CountryID)?.CountryName;
            return personResponse;

        }

        public bool DeletePerson(Guid? ID)
        {
            if (ID == null) throw new ArgumentNullException();
            if (ID == Guid.Empty) throw new ArgumentNullException(nameof(ID));
            if (_people.RemoveAll(temp => temp.PersonID == ID) > 0)
                return true;
            else
                return false;
        }

        public List<PersonResponse> GetAllPeople()
        {
            return _people.Select((temp) => temp.ToPersonResponse()).ToList();
        }
        public List<PersonResponse> GetFiltered(string? SearchString, string? SearchBy)
        {
            SearchString = SearchString?.Trim();
            List<PersonResponse> All = GetAllPeople();
            List<PersonResponse> Filtered = GetAllPeople();
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

        public PersonResponse? GetPersonById(Guid? ID)
        {
            if (ID == null || ID == Guid.Empty) throw new ArgumentNullException("Id is Null");
            return _people.FirstOrDefault((temp) => temp.PersonID == ID)?.ToPersonResponse();
        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
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

        public PersonResponse UpdatePerson(UpdatePersonRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(personUpdateRequest));
            ValidationHelper.ModelValidator(personUpdateRequest);
            Person? person = _people.Where(temp => temp.PersonID == personUpdateRequest.PersonId).FirstOrDefault();
            if (person == null)
                throw new ArgumentException("This ID does not exist in your Contacts");
            person.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            person.Gender = !string.IsNullOrEmpty(personUpdateRequest.Gender) ? (GenderOptions)Enum.Parse(typeof(GenderOptions), personUpdateRequest.Gender) : null;
            person.Email = personUpdateRequest.Email;
            person.DateOfBirth = personUpdateRequest.DateOfBirth;
            person.Address = personUpdateRequest.Address;
            person.CountryID = personUpdateRequest.CountryId;

            return person.ToPersonResponse();

        }
    }
}
