using AutoFixture;
using Entities;
using Entities.Enums;
using EntityFrameworkCoreMock;
using FluentAssertions;
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
        private readonly IFixture fixture;
        public PersonServicesTest(ITestOutputHelper testOutputHelper)
        {
            fixture = new Fixture();
            _outputHelper = testOutputHelper;
            var countriesInitialData = new List<Country>() { };
            var personsInitialData = new List<Person>() { };

            //Craete mock for DbContext
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
              new DbContextOptionsBuilder<ApplicationDbContext>().Options
             );

            ApplicationDbContext dbContext = dbContextMock.Object;

            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

            _countryService = new CountryServices(dbContext);
            _personService = new PersonServices(dbContext, _countryService);

        }
        #region AddPerson
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            AddPersonRequest? addPersonRequest = null;
            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            //{
            //    await _personService.AddPerson(addPersonRequest);
            //});
            Func<Task> action = async () =>
            {
                await _personService.AddPerson(addPersonRequest);
            };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }
        [Fact]
        public async Task AddPerson_EmptyName_Email()
        {
            AddPersonRequest addPersonRequest = new AddPersonRequest() { Email = null, PersonName = null };
            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            //{
            //    await _personService.AddPerson(addPersonRequest);
            //});
            Func<Task> action = async () =>
            {
                await _personService.AddPerson(addPersonRequest);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }
        [Fact]
        public async Task AddPerson_DuplicateNameAndEmail()
        {
            AddPersonRequest addPersonRequest1 = new AddPersonRequest() { Email = "aelsayed@gmail.com", PersonName = "  Ahmed Elsayed", Gender = "Male" };
            AddPersonRequest addPersonRequest2 = new AddPersonRequest() { Email = "aelsayed@gmail.com", PersonName = "Ahmed Elsayed", Gender = "Male" };
            ////await Assert.ThrowsAsync<ArgumentException>(async () =>
            ////{
            ////    await _personService.AddPerson(addPersonRequest2);
            ////    await _personService.AddPerson(addPersonRequest1);
            ////});
            Func<Task> action = async () =>
            {
                await _personService.AddPerson(addPersonRequest2);
                await _personService.AddPerson(addPersonRequest1);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }
        [Fact]
        public async Task AddPerson_Normal()
        {
            AddPersonRequest addPersonRequest = fixture.Build<AddPersonRequest>()
                 .With(t => t.Email, "asdasd@gmial.com")
                 .With(t => t.Gender, "Male")
                 .Create();
            PersonResponse response = await _personService.AddPerson(addPersonRequest);
            //Assert.True(response.PersonID != Guid.Empty && response.PersonName != string.Empty && response.Email != String.Empty);
            response.PersonID.Should().NotBe(Guid.Empty);
        }
        #endregion
        #region GetPersonById
        [Fact]
        public async Task GetPersonById_nullId()
        {
            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            //{
            //    await _personService.GetPersonById(Guid.Empty);
            //});
            Func<Task> action =(async () =>
            {
                await _personService.GetPersonById(Guid.Empty);
            });
           await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetPersonById_Normal()
        {
            AddCountryRequest addCountryRequest = new AddCountryRequest() { CountryName = "USA" };
            CountryResponse countryResponse = await _countryService.AddCountry(addCountryRequest);
            AddPersonRequest addPersonRequest = fixture.Build<AddPersonRequest>()
                .With(t => t.Email, "asdasd@gmial.com")
                .With(t => t.Gender, "Male")
                .With(t=>t.CountryId, countryResponse.CountryId)
                .With(t=>t.Country,"fff")
                .Create();
            PersonResponse? personResponse_Add = await _personService.AddPerson(addPersonRequest);
            PersonResponse? personResponse_Get = await _personService.GetPersonById(personResponse_Add.PersonID);
            Assert.Equal(personResponse_Add, personResponse_Get);
            //personResponse_Add.Should().Be(personResponse_Get);
        }
        #endregion
        #region GetAllPeople
        [Fact]
        public async Task GetAllpeople_EmptyList()
        {
            List<PersonResponse> AllContacts = await _personService.GetAllPeople();
            //Assert.Empty(AllContacts);
            AllContacts.Should().BeEmpty();
        }
        [Fact]
        public async Task GetAllpeople_Normal()
        {
            AddCountryRequest country_request_1 = fixture.Create<AddCountryRequest>();
            AddCountryRequest country_request_2 = fixture.Create<AddCountryRequest>();

            CountryResponse country_response_1 = await _countryService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countryService.AddCountry(country_request_2);
            AddPersonRequest person_request_1 = fixture.Build<AddPersonRequest>()
   .With(temp => temp.Email, "someone_1@example.com")
   .With(t => t.Gender, GenderOptions.Male.ToString())
   .Create();

            AddPersonRequest person_request_2 = fixture.Build<AddPersonRequest>()
   .With(temp => temp.Email, "someone_1@example.com")
   .With(t => t.Gender, GenderOptions.Male.ToString())
   .Create();

            AddPersonRequest person_request_3 = fixture.Build<AddPersonRequest>()
   .With(temp => temp.Email, "someone_1@example.com")
   .With(t => t.Gender, GenderOptions.Male.ToString())
   .Create();
            List<AddPersonRequest> list = new List<AddPersonRequest>() { person_request_1, person_request_2, person_request_3 };
            List<PersonResponse> list_Add = new List<PersonResponse>();
            foreach (AddPersonRequest addPersonRequest in list)
            {
                list_Add.Add(await _personService.AddPerson(addPersonRequest));
            }
            List<PersonResponse> list_get = await _personService.GetAllPeople();
            foreach (PersonResponse personResponse in list_Add)
            {
                //Assert.Contains(personResponse, list_get);
                list_get.Should().Contain(personResponse);
            }
        }
        #endregion
        #region GetFiltered
        [Fact]
        public async Task GetPeople_EmptySearchText()
        {
            AddCountryRequest country_request_1 = fixture.Create<AddCountryRequest>();
            AddCountryRequest country_request_2 = fixture.Create<AddCountryRequest>();

            CountryResponse country_response_1 = await _countryService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countryService.AddCountry(country_request_2);
            AddPersonRequest person_request_1 = fixture.Build<AddPersonRequest>()
           .With(temp => temp.Email, "someone_1@example.com")
           .With(t => t.Gender, GenderOptions.Male.ToString())
           .Create();

            AddPersonRequest person_request_2 = fixture.Build<AddPersonRequest>()
           .With(temp => temp.Email, "someone_1@example.com")
           .With(t => t.Gender, GenderOptions.Male.ToString())
           .Create();

            AddPersonRequest person_request_3 = fixture.Build<AddPersonRequest>()
           .With(temp => temp.Email, "someone_1@example.com")
           .With(t => t.Gender, GenderOptions.Male.ToString())
           .Create();
            List<AddPersonRequest> list = new List<AddPersonRequest>() { person_request_1, person_request_2, person_request_3 };
            List<PersonResponse> list_Add = new List<PersonResponse>();
            foreach (AddPersonRequest addPersonRequest in list)
            {
                list_Add.Add(await _personService.AddPerson(addPersonRequest));
            }
            List<PersonResponse> filtered = await _personService.GetFiltered(string.Empty, "Person Name");
            foreach (PersonResponse personResponse in list_Add)
            {
                filtered.Should().Contain(personResponse);
            }
        }
        [Fact]
        public async Task GetPeople_Normal()
        {
            AddCountryRequest country_request_1 = fixture.Create<AddCountryRequest>();
            AddCountryRequest country_request_2 = fixture.Create<AddCountryRequest>();

            CountryResponse country_response_1 = await _countryService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countryService.AddCountry(country_request_2);
            AddPersonRequest person_request_1 = fixture.Build<AddPersonRequest>()
           .With(temp => temp.Email, "someone_1@example.com")
           .With(t => t.Gender, GenderOptions.Male.ToString())
           .Create();

            AddPersonRequest person_request_2 = fixture.Build<AddPersonRequest>()
           .With(temp => temp.Email, "someone_1@example.com")
           .With(t => t.Gender, GenderOptions.Male.ToString())
           .Create();

            AddPersonRequest person_request_3 = fixture.Build<AddPersonRequest>()
           .With(temp => temp.Email, "someone_1@example.com")
           .With(t => t.Gender, GenderOptions.Male.ToString())
           .Create();
            List<AddPersonRequest> list = new List<AddPersonRequest>() { person_request_1, person_request_2, person_request_3 };
            List<PersonResponse> list_Add = new List<PersonResponse>();
            foreach (AddPersonRequest addPersonRequest in list)
            {
                list_Add.Add(await _personService.AddPerson(addPersonRequest));
            }
            List<PersonResponse> filtered = await _personService.GetFiltered("   someone ", nameof(Person.Email));
            //foreach (PersonResponse personResponse in filtered)
            //{
            //    if (personResponse.PersonName.Contains("at", StringComparison.OrdinalIgnoreCase))
            //    {
            //        list_Add.Should().Contain(personResponse);
            //    }
            //}
            list_Add.Should().BeEquivalentTo(filtered);
        }
        #endregion
        #region UpdatePerson
        [Fact]
        public async Task UpdatePerson_EmptyName_ID()
        {
            AddCountryRequest countryRequest = new AddCountryRequest() { CountryName = "EGY" };
            CountryResponse countryResponse = await _countryService.AddCountry(countryRequest);
            AddPersonRequest addPersonRequest = fixture.Build<AddPersonRequest>()
                .With(t => t.Email, "email@gmail.com")
                .With(t => t.CountryId, countryResponse.CountryId)
                .With(t => t.Gender, "Male")
                .Create();
            UpdatePersonRequest updatePersonRequest = (await _personService.AddPerson(addPersonRequest)).ToPersonUpdateRequest();
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
            AddPersonRequest addPersonRequest = fixture.Build<AddPersonRequest>()
               .With(t => t.Email, "email@gmail.com")
               .With(t => t.CountryId, countryResponse.CountryId)
               .With(t => t.Gender, "Male")
               .Create();
            UpdatePersonRequest updatePersonRequest = (await _personService.AddPerson(addPersonRequest)).ToPersonUpdateRequest();
            updatePersonRequest.PersonId = Guid.NewGuid();
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personService.UpdatePerson(updatePersonRequest);
            });

        }
        [Fact]
        public async Task UpdatePerson_Normal()
        {
            AddCountryRequest countryRequest = new AddCountryRequest() { CountryName = "EGY" };
            CountryResponse countryResponse = await _countryService.AddCountry(countryRequest);
            AddPersonRequest addPersonRequest = fixture.Build<AddPersonRequest>()
               .With(t => t.Email, "email@gmail.com")
               .With(t => t.CountryId, countryResponse.CountryId)
               .With(t => t.Gender, "Male")
               .Create();
            UpdatePersonRequest updatePersonRequest = (await _personService.AddPerson(addPersonRequest)).ToPersonUpdateRequest();
            updatePersonRequest.PersonName = "Sayed Ahmed";
            updatePersonRequest.Email = "aelsayed777888@gmail.com";
            PersonResponse? personResponse_fromUpdate = await _personService.UpdatePerson(updatePersonRequest);
            PersonResponse? personResponse_fromGet = await _personService.GetPersonById(updatePersonRequest.PersonId);
            _outputHelper.WriteLine(personResponse_fromGet?.ToString());
            _outputHelper.WriteLine(personResponse_fromUpdate?.ToString());

            Assert.Equal(personResponse_fromGet, personResponse_fromUpdate);
           // personResponse_fromGet.Should().Be(personResponse_fromUpdate);

        }

        #endregion
    }
}
