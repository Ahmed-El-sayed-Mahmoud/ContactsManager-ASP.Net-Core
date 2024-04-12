using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Xunit.Abstractions;
using Entities;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using Moq;
using RepositoryContracts;
using FluentAssertions;

namespace TestProject
{
    public class CountryServicesTest
    {
        private readonly ICountryServices _countryService;
        private readonly ITestOutputHelper _outputHelper;
        private readonly Mock<ICountryRepository> _countryRepositoryMock;
        public CountryServicesTest(ITestOutputHelper testOutputHelper)
        {
            _countryRepositoryMock = new Mock<ICountryRepository>();
            ICountryRepository countryRepository = _countryRepositoryMock.Object;
            _countryService = new CountryServices(countryRepository);

            //List<Country> SeedData = new List<Country>();
            //DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
            //   new DbContextOptionsBuilder<ApplicationDbContext>().Options);
            //var dbContext= dbContextMock.Object;
            //_countryService=new CountryServices(countryRepository);
            //dbContextMock.CreateDbSetMock<Country>(t => t.Countries,SeedData);
            _outputHelper = testOutputHelper;
        }
        #region AddCountry
        [Fact]
        public async Task AddCountry_NullArg()
        {
            AddCountryRequest? request = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            {
                await _countryService.AddCountry(request);
            });
        }
        [Fact]
        public async Task AddCountry_NullCountryName()
        {
            AddCountryRequest request = new AddCountryRequest() { CountryName = null };
           await Assert.ThrowsAsync<ArgumentException>(async () => {await  _countryService.AddCountry(request); });
        }
        [Fact]
        public async Task AddCountry_DuplicateCountries()
        {
            AddCountryRequest req1 = new AddCountryRequest() { CountryName = "USA" };
            _countryRepositoryMock.Setup(t=>t.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(req1.ToCountry());
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countryService.AddCountry(req1);
                await  _countryService.AddCountry(req1);
            });
        }
        [Fact]
        public async Task AddCountry_ProperCountry()
        {
            AddCountryRequest req1 = new AddCountryRequest() { CountryName = "USA" };
            _countryRepositoryMock.Setup(t => t.AddCountry(It.IsAny<Country>())).ReturnsAsync(1);
            CountryResponse res = await _countryService.AddCountry(req1);
            Assert.True(res.CountryId != Guid.Empty);
        }
        #endregion
        #region GetCountires
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            List<Country> list = new List<Country>();
            _countryRepositoryMock.Setup(t=>t.GetAllCountries()).ReturnsAsync(list);
            List<CountryResponse> Expected=list.Select(t=>t.ToCountryResponse()).ToList();
            List<CountryResponse> Actual=await _countryService.GetAllCountries();

            Assert.Empty(Actual);
        }
        [Fact]
        public async Task GetAllCountries_Normal()
        {
            List<Country> list = new List<Country>()
            { 
                new Country(){CountryName="China" , CountryID=Guid.NewGuid()},
                new Country(){CountryName="USA" , CountryID=Guid.NewGuid()},
            };
            _countryRepositoryMock.Setup(t => t.GetAllCountries()).ReturnsAsync(list);
            List<CountryResponse> Expected = list.Select(t => t.ToCountryResponse()).ToList();
            List<CountryResponse> Actual = await _countryService.GetAllCountries();

            Actual.Should().BeEquivalentTo(Expected);
        }
        #endregion
        #region GetCountryById
        [Fact]
        public async Task GetCountryById_EmptyId()
        {
            Guid guid = Guid.Empty;
            Assert.Null(await _countryService.GetCountryById(guid));
        }
        [Fact]
        public async Task GetCountryById_NoMatch()
        {
            CountryResponse? res = await _countryService.GetCountryById(Guid.NewGuid());
            Assert.Null(res);
        }
        [Fact]
        public async Task GetCountryById_Normal()
        {
            Country country = new Country() { CountryName = "China", CountryID = Guid.NewGuid() };
            _countryRepositoryMock.Setup(t=>t.GetCountryById(It.IsAny<Guid>())).ReturnsAsync(country);
            CountryResponse? Actual= await _countryService.GetCountryById(country.CountryID);
            CountryResponse Expected= country.ToCountryResponse();
            Assert.Equal(Expected, await _countryService.GetCountryById(Expected.CountryId));
            //Actual.Should().Be(Expected);
        }
        #endregion

    }
}