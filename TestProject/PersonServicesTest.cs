using AutoFixture;
using Entities;
using Entities.Enums;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Xunit.Abstractions;
using Moq;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Serilog;
using Exceptions;

namespace TestProject
{
    public class PersonServicesTest
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly IPersonGetterServices _personGetterService;
		private readonly IPersonAdderServices _personAdderService;
		private readonly IPersonDeleterServices _personDeleterService;
		private readonly IPersonUpdaterServices _personUpdaterService;
		private readonly IPersonUploaderServices _personUploaderService;
		private readonly IPersonSorterServices _personSorterService;
		private readonly IFixture fixture;
        private readonly Mock<IPersonRepository> _personRepositoryMock;
        public PersonServicesTest(ITestOutputHelper testOutputHelper)
        {
            fixture = new Fixture();
            _personRepositoryMock = new Mock<IPersonRepository>();
            IPersonRepository personRepository = _personRepositoryMock.Object;
            _outputHelper = testOutputHelper;
            //var countriesInitialData = new List<Country>() { };
            //var personsInitialData = new List<Person>() { };

            //Craete mock for DbContext
            //DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
            //  new DbContextOptionsBuilder<ApplicationDbContext>().Options
            // );

            //ApplicationDbContext dbContext = dbContextMock.Object;

            //dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            //dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);
            Mock<ILogger<PersonGetterServices>>  LoggerMock = new Mock<ILogger<PersonGetterServices>>();
            Mock<IDiagnosticContext> DiagnosticMock=new Mock<IDiagnosticContext>();

            _personGetterService = new PersonGetterServices(personRepository,LoggerMock.Object,DiagnosticMock.Object);
			_personAdderService = new PersonAdderServices(personRepository, LoggerMock.Object, DiagnosticMock.Object);
			_personUpdaterService = new PersonUpdaterServices(personRepository, LoggerMock.Object, DiagnosticMock.Object);
			_personUploaderService = new PersonUploaderServices(personRepository, LoggerMock.Object, DiagnosticMock.Object,_personAdderService);
			_personDeleterService = new PersonDeleterServices(personRepository, LoggerMock.Object, DiagnosticMock.Object);
			_personSorterService = new PersonSorterServices(personRepository, LoggerMock.Object, DiagnosticMock.Object);
		

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
                await _personAdderService.AddPerson(addPersonRequest);
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
                await _personAdderService.AddPerson(addPersonRequest);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }
        [Fact]
        public async Task AddPerson_DuplicateNameAndEmail()
        {
            AddPersonRequest addPersonRequest1 = new AddPersonRequest() { Email = "aelsayed@gmail.com", PersonName = "  Ahmed Elsayed", Gender = "Male" };
            Person person = fixture.Build<Person>()
                .With(t => t.Email, "aelsayed@gmail.com")
                .With(t=>t.PersonName, "Ahmed Elsayed")
                .With(t => t.Gender, "Male")
                .Create();
            List<Person> list=new List<Person> { person };
            _personRepositoryMock.Setup(t=>t.GetAllPeople()).ReturnsAsync(list);
            ////await Assert.ThrowsAsync<ArgumentException>(async () =>
            ////{
            ////    await _personService.AddPerson(addPersonRequest2);
            ////    await _personService.AddPerson(addPersonRequest1);
            ////});
            Func<Task> action = async () =>
            {
                await _personAdderService.AddPerson(addPersonRequest1);
                await _personAdderService.AddPerson(addPersonRequest1);
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
            _personRepositoryMock.Setup(t => t.AddPerson(It.IsAny<Person>()))
                                 .ReturnsAsync(1);

            PersonResponse response = await _personAdderService.AddPerson(addPersonRequest);
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
                await _personGetterService.GetPersonById(Guid.Empty);
            });
           await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetPersonById_Normal()
        {
         
           Person person = fixture.Build<Person>()
                .With(t => t.Email, "asdasd@gmial.com")
                .With(t => t.Gender, "Male")
                .Create();
            _personRepositoryMock.Setup(t => t.GetPersonById(It.IsAny<Guid>()))
            .ReturnsAsync(person);
            PersonResponse Expected=person.ToPersonResponse();
            PersonResponse? Actual=await _personGetterService.GetPersonById(Guid.NewGuid());
            Assert.Equal(Expected, Actual);
            //personResponse_Add.Should().Be(personResponse_Get);
        }
        #endregion
        #region GetAllPeople
        [Fact]
        public async Task GetAllpeople_EmptyList()
        {
          
            _personRepositoryMock.Setup(t=>t.GetAllPeople()).ReturnsAsync(new List<Person>());
            List<PersonResponse> Actual = await _personGetterService.GetAllPeople();
            //Assert.Empty(AllContacts);
            Actual.Should().BeEmpty();
        }
        [Fact]
        public async Task GetAllpeople_Normal()
        {
            List<Person> list = new List<Person>(){
                fixture.Build<Person>().With(t=>t.Email,"someone1@gmail.com").With(t => t.Gender, "Male").Create(),
                fixture.Build<Person>().With(t=>t.Email,"someone2@gmail.com").With(t => t.Gender, "Male").Create(),

            };
            _personRepositoryMock.Setup(t => t.GetAllPeople()).ReturnsAsync(list);
            List<PersonResponse> Expected = list.Select(t => t.ToPersonResponse()).ToList();
            List<PersonResponse> Actual = await _personGetterService.GetAllPeople();
            //Assert.Empty(AllContacts);
            Actual.Should().BeEquivalentTo(Expected);
        }
        #endregion
        #region GetFiltered
        [Fact]
        public async Task GetPeople_EmptySearchText()
        {

            List<Person> list = new List<Person>(){
                fixture.Build<Person>().With(t=>t.Email,"someone1@gmail.com").With(t => t.Gender, "Male").Create(),
                fixture.Build<Person>().With(t=>t.Email,"someone2@gmail.com").With(t => t.Gender, "Male").Create(),

            };
            _personRepositoryMock.Setup(t=>t.GetFiltered(It.IsAny<Expression<Func<Person,bool>>>())).ReturnsAsync(list);
            List<PersonResponse> Expected = list.Select(t => t.ToPersonResponse()).ToList();
            List<PersonResponse> Actual = await _personGetterService.GetFiltered("",nameof(Person.Email));
            //Assert.Empty(AllContacts);
            Actual.Should().BeEquivalentTo(Expected);
        }
        [Fact]
        public async Task GetPeople_Normal()
        {
            List<Person> list = new List<Person>(){
                fixture.Build<Person>().With(t=>t.Email,"someone1@gmail.com").With(t => t.Gender, "Male").Create(),
                fixture.Build<Person>().With(t=>t.Email,"someone2@gmail.com").With(t => t.Gender, "Male").Create(),
                fixture.Build<Person>().With(t=>t.Email,"noone@gmail.com").With(t => t.Gender, "Male").Create(),

            };
            List<Person> Expected = list.Where(t => t.Email.Contains("someone", StringComparison.OrdinalIgnoreCase)).ToList();
            _personRepositoryMock.Setup(t => t.GetFiltered(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(Expected);
            List<PersonResponse>ExpectedResponse=Expected.Select(t=>t.ToPersonResponse()).ToList();
            List<PersonResponse> Actual = await _personGetterService.GetFiltered("someone",nameof(Person.Email));
            //Assert.Empty(AllContacts);
            Actual.Should().BeEquivalentTo(ExpectedResponse);
           
        }
        #endregion
        #region UpdatePerson
        [Fact]
        public async Task UpdatePerson_EmptyName_ID()
        {

            UpdatePersonRequest updatePersonRequest = fixture.Build<UpdatePersonRequest>()
                                                     .With(t => t.PersonId, Guid.Empty)
                                                     .With(t=>t.Email,"ash@gmail.com").Create();
            await Assert.ThrowsAsync<InvalidIDException>(async () =>
            {
                await _personUpdaterService.UpdatePerson(updatePersonRequest);
            });

        }
        [Fact]
        public async Task UpdatePerson_InvalidID()
        {
            UpdatePersonRequest updatePersonRequest = fixture.Build<UpdatePersonRequest>()
                                                     .With(t => t.Email, "ash@gmail.com").Create();
            Person? person=null;
            _personRepositoryMock.Setup(t => t.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);
            await Assert.ThrowsAsync<InvalidIDException>(async () =>
            {
                await _personUpdaterService.UpdatePerson(updatePersonRequest);
            });

        }
        [Fact]
        public async Task UpdatePerson_Normal()
        {
            UpdatePersonRequest updatePersonRequest = fixture.Build<UpdatePersonRequest>()
                                                    .With(t => t.Email, "ash@gmail.com").
                                                    With(t=>t.Gender,"Male").Create();
            Person? person= updatePersonRequest.ToPerson();
            _personRepositoryMock.Setup(t => t.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);
            _personRepositoryMock.Setup(t => t.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(1);
            PersonResponse Expected=person.ToPersonResponse();
            PersonResponse Actual = await _personUpdaterService.UpdatePerson(updatePersonRequest);
            Assert.Equal(Expected,Actual );
           // personResponse_fromGet.Should().Be(personResponse_fromUpdate);

        }

        #endregion
    }
}
