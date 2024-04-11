using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Xunit.Abstractions;
using Entities;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;

namespace TestProject
{
    public class CountryServicesTest
    {
        private readonly ICountryServices _countryService;
        private readonly ITestOutputHelper _outputHelper;
        public CountryServicesTest(ITestOutputHelper testOutputHelper)
        {
            List<Country> SeedData = new List<Country>();
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
               new DbContextOptionsBuilder<ApplicationDbContext>().Options);
            var dbContext= dbContextMock.Object;
            _countryService=new CountryServices(null);
            dbContextMock.CreateDbSetMock<Country>(t => t.Countries,SeedData);
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
            AddCountryRequest req2 = new AddCountryRequest() { CountryName = "USA" };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countryService.AddCountry(req2);
                await  _countryService.AddCountry(req1);
            });
        }
        [Fact]
        public async Task AddCountry_ProperCountry()
        {
            AddCountryRequest req1 = new AddCountryRequest() { CountryName = "USA" };
            CountryResponse res = await _countryService.AddCountry(req1);
            Assert.True(res.CountryId != Guid.Empty);
        }
        #endregion
        #region GetCountires
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            List<CountryResponse> actual_country_response_list = await _countryService.GetAllCountries();

            Assert.Empty(actual_country_response_list);
        }
        [Fact]
        public async Task GetAllCountries_Normal()
        {
            List<AddCountryRequest> testList = new List<AddCountryRequest>() { new AddCountryRequest() { CountryName="USA"},
            new AddCountryRequest (){ CountryName="EGY"} };
            List<CountryResponse> ExpectedRes = new List<CountryResponse>();
            foreach (AddCountryRequest test in testList)
            {
                ExpectedRes.Add(await _countryService.AddCountry(test));
            }
            List<CountryResponse> actual_country_response = await _countryService.GetAllCountries();
            foreach (CountryResponse res in actual_country_response)
            {
                Assert.Contains(res, ExpectedRes);
            }
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
            AddCountryRequest addCountryRequest = new AddCountryRequest() { CountryName = "GER" };
            CountryResponse? Expected = await _countryService.AddCountry(addCountryRequest);
            _outputHelper.WriteLine(Expected.CountryId.ToString());
            _outputHelper.WriteLine((await _countryService.GetCountryById(Expected.CountryId))?.CountryId.ToString());
            Assert.Equal(Expected, await _countryService.GetCountryById(Expected.CountryId));
        }
        #endregion

    }
}