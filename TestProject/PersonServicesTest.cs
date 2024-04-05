using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Xunit.Abstractions;

namespace TestProject
{
    public class PersonServicesTest
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly IPersonServices _personService;
        private readonly ICountryServices _countryService;
        public PersonServicesTest(ITestOutputHelper testOutputHelper)
        {
            _outputHelper = testOutputHelper;
            _countryService = new CountryServices(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
            _personService = new PersonServices(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options), _countryService);

        }
        #region AddPerson
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            AddPersonRequest? addPersonRequest = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            {
                await _personService.AddPerson(addPersonRequest);
            });
        }
        [Fact]
        public async Task AddPerson_EmptyName_Email()
        {
            AddPersonRequest addPersonRequest = new AddPersonRequest() { Email = null, PersonName = null };
           await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                await _personService.AddPerson(addPersonRequest);
            });
        }
        [Fact]
        public async Task AddPerson_DuplicateNameAndEmail()
        {
            AddPersonRequest addPersonRequest1 = new AddPersonRequest() { Email = "aelsayed@gmail.com", PersonName = "  Ahmed Elsayed" };
            AddPersonRequest addPersonRequest2 = new AddPersonRequest() { Email = "aelsayed@gmail.com", PersonName = "Ahmed Elsayed" };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personService.AddPerson(addPersonRequest2);
               await  _personService.AddPerson(addPersonRequest1);
            });
        }
        [Fact]
        public async Task AddPerson_Normal()
        {
            AddPersonRequest addPersonRequest = new AddPersonRequest()
            {
                Email = "aelsayed@gmail.com",
                PersonName = "Ahmed Elsayed",
                Gender = "Male",
                DateOfBirth = new DateTime(2003, 11, 9),
                ReceiveNewsLetters = true,
                CountryId = Guid.NewGuid(),
            };
            PersonResponse response =await _personService.AddPerson(addPersonRequest);
            Assert.True(response.PersonID != Guid.Empty && response.PersonName != string.Empty && response.Email != String.Empty);
        }
        #endregion
        #region GetPersonById
        [Fact]
        public async Task GetPersonById_nullId()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _personService.GetPersonById(Guid.Empty);
            });
        }

        [Fact]
        public async Task GetPersonById_Normal()
        {
            AddCountryRequest addCountryRequest = new AddCountryRequest() { CountryName = "USA" };
            CountryResponse countryResponse =await _countryService.AddCountry(addCountryRequest);
            AddPersonRequest addPersonRequest = new AddPersonRequest()
            {
                PersonName = "Ahmed Elsayed ",
                Email = "aelsayed@gmail.com"
            ,
                CountryId = countryResponse.CountryId,
                DateOfBirth = DateTime.Parse("11-9-2003"),
                Gender = "Male"
            };
            PersonResponse? personResponse_Add = await _personService.AddPerson(addPersonRequest);
            PersonResponse? personResponse_Get = await _personService.GetPersonById(personResponse_Add.PersonID);
            Assert.Equal(personResponse_Add, personResponse_Get);
        }
        #endregion
        #region GetAllPeople
        [Fact]
        public async Task GetAllpeople_EmptyList()
        {
            List<PersonResponse> AllContacts =await _personService.GetAllPeople();
            Assert.Empty(AllContacts);
        }
        [Fact]
        public async Task GetAllpeople_Normal()
        {
            AddCountryRequest country_request_1 = new AddCountryRequest() { CountryName = "USA" };
            AddCountryRequest country_request_2 = new AddCountryRequest() { CountryName = "GER" };

            CountryResponse country_response_1 = await _countryService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countryService.AddCountry(country_request_2);
            List<AddPersonRequest> list = new List<AddPersonRequest>() { new AddPersonRequest() { PersonName = "Ahmed Elsayed", Email = "aboelseed@gmail.com"
            , DateOfBirth=DateTime.Parse("11-9-2003"), Gender="Male" , CountryId= country_response_1.CountryId},
            new AddPersonRequest() { PersonName = "Fatma Moustafa", Email = "Fmoustafa@gmail.com" , DateOfBirth=DateTime.Parse("1975-.08.07")
             , CountryId= country_response_2.CountryId , Gender = "Female"} };
            List<PersonResponse> list_Add = new List<PersonResponse>();
            foreach (AddPersonRequest addPersonRequest in list)
            {
                list_Add.Add(await _personService.AddPerson(addPersonRequest));
            }
            List<PersonResponse> list_get = await _personService.GetAllPeople();
            foreach (PersonResponse personResponse in list_Add)
            {
                Assert.Contains(personResponse, list_get);
            }
        }
        #endregion
        #region GetFiltered
        [Fact]
        public async Task GetPeople_EmptySearchText()
        {
            AddCountryRequest country_request_1 = new AddCountryRequest() { CountryName = "USA" };
            AddCountryRequest country_request_2 = new AddCountryRequest() { CountryName = "GER" };

            CountryResponse country_response_1 = await _countryService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countryService.AddCountry(country_request_2);
            List<AddPersonRequest> list = new List<AddPersonRequest>() { new AddPersonRequest() { PersonName = "Ahmed Elsayed", Email = "aboelseed@gmail.com"
            , DateOfBirth=DateTime.Parse("11-9-2003"), Gender="Male" , CountryId= country_response_1.CountryId},
            new AddPersonRequest() { PersonName = "Fatma Moustafa", Email = "Fmoustafa@gmail.com" , DateOfBirth=DateTime.Parse("1975-.08.07")
             , CountryId= country_response_2.CountryId , Gender = "Female"} };
            List<PersonResponse> list_Add = new List<PersonResponse>();
            foreach (AddPersonRequest addPersonRequest in list)
            {
                list_Add.Add( await _personService.AddPerson(addPersonRequest));
            }
            List<PersonResponse> filtered =  await _personService.GetFiltered(string.Empty, "Person Name");
            foreach (PersonResponse personResponse in list_Add)
            {
                Assert.Contains(personResponse, filtered);
            }
        }
        [Fact]
        public async Task GetPeople_Normal()
        {
            AddCountryRequest country_request_1 = new AddCountryRequest() { CountryName = "USA" };
            AddCountryRequest country_request_2 = new AddCountryRequest() { CountryName = "GER" };

            CountryResponse country_response_1 = await _countryService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countryService.AddCountry(country_request_2);
            List<AddPersonRequest> list = new List<AddPersonRequest>() { new AddPersonRequest() { PersonName = "Ahmed Elsatyed", Email = "aboelseed@gmail.com"
            , DateOfBirth=DateTime.Parse("11-9-2003"), Gender="Male" , CountryId= country_response_1.CountryId},
            new AddPersonRequest() { PersonName = "Fatma Moustafa", Email = "Fmoustafa@gmail.com" , DateOfBirth=DateTime.Parse("1975-.08.07")
             , CountryId= country_response_2.CountryId , Gender = "Female"} };
            List<PersonResponse> list_Add = new List<PersonResponse>();
            foreach (AddPersonRequest addPersonRequest in list)
            {
                list_Add.Add( await _personService.AddPerson(addPersonRequest));
            }
            List<PersonResponse> filtered = await _personService.GetFiltered("   at ", nameof(Person.PersonName));
            foreach (PersonResponse personResponse in list_Add)
            {
                if (personResponse.PersonName.Contains("at", StringComparison.OrdinalIgnoreCase))
                {
                    Assert.Contains(personResponse, filtered);
                }
            }
        }
        #endregion
        #region UpdatePerson
        [Fact]
        public async Task UpdatePerson_EmptyName_ID()
        {
            AddCountryRequest countryRequest = new AddCountryRequest() { CountryName = "EGY" };
            CountryResponse countryResponse = await _countryService.AddCountry(countryRequest);
            AddPersonRequest addPersonRequest = new AddPersonRequest() { Email = "aelsayed@gmail.com", PersonName = "ahmed", CountryId = countryResponse.CountryId };
            UpdatePersonRequest updatePersonRequest = ( await _personService.AddPerson(addPersonRequest)).ToPersonUpdateRequest();
            updatePersonRequest.PersonName = string.Empty;
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
               await _personService.UpdatePerson(updatePersonRequest);
            });

        }
        [Fact]
        public async Task UpdatePerson_InvalidID()
        {
            AddCountryRequest countryRequest = new AddCountryRequest() { CountryName = "EGY" };
            CountryResponse countryResponse = await _countryService.AddCountry(countryRequest);
            AddPersonRequest addPersonRequest = new AddPersonRequest() { Email = "aelsayed@gmail.com", PersonName = "ahmed", CountryId = countryResponse.CountryId };
            UpdatePersonRequest updatePersonRequest = (await _personService.AddPerson(addPersonRequest)).ToPersonUpdateRequest();
            updatePersonRequest.PersonId = Guid.NewGuid();
            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                 await _personService.UpdatePerson(updatePersonRequest);
            });

        }
        [Fact]
        public async Task UpdatePerson_Normal()
        {
            AddCountryRequest countryRequest = new AddCountryRequest() { CountryName = "EGY" };
            CountryResponse countryResponse =  await _countryService.AddCountry(countryRequest);
            AddPersonRequest addPersonRequest = new AddPersonRequest()
            {
                Email = "aelsayed@gmail.com",
                PersonName = "ahmed",
                CountryId = countryResponse.CountryId
            ,
            };
            UpdatePersonRequest updatePersonRequest = (await _personService.AddPerson(addPersonRequest)).ToPersonUpdateRequest();
            updatePersonRequest.PersonName = "Sayed Ahmed";
            updatePersonRequest.Email = "aelsayed777888@gmail.com";
            PersonResponse? personResponse_fromUpdate = await _personService.UpdatePerson(updatePersonRequest);
            PersonResponse? personResponse_fromGet = await _personService.GetPersonById(updatePersonRequest.PersonId);
            _outputHelper.WriteLine(personResponse_fromGet?.ToString());
            _outputHelper.WriteLine(personResponse_fromUpdate?.ToString());

            Assert.Equal(personResponse_fromGet, personResponse_fromUpdate);

        }

        #endregion
    }
}
