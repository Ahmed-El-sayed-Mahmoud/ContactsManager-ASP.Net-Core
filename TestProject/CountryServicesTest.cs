using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Xunit.Abstractions;
using Entities;

namespace TestProject
{
    public class CountryServicesTest
    {
        private readonly ICountryServices _countryService;
        private readonly ITestOutputHelper _outputHelper;
        public CountryServicesTest(ITestOutputHelper testOutputHelper)
        {
            //_countryService = new CountryServices(PersonsDbContext personsDbContext);
            //_outputHelper = testOutputHelper;
        }
        #region AddCountry
        [Fact]
        public void AddCountry_NullArg()
        {
            AddCountryRequest? request = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                _countryService.AddCountry(request);
            });
        }
        [Fact]
        public void AddCountry_NullCountryName()
        {
            AddCountryRequest request = new AddCountryRequest() { CountryName = null };
            Assert.Throws<ArgumentException>(() => { _countryService.AddCountry(request); });
        }
        [Fact]
        public void AddCountry_DuplicateCountries()
        {
            AddCountryRequest req1 = new AddCountryRequest() { CountryName = "USA" };
            AddCountryRequest req2 = new AddCountryRequest() { CountryName = "USA" };
            Assert.Throws<ArgumentException>(() =>
            {
                _countryService.AddCountry(req2);
                _countryService.AddCountry(req1);
            });
        }
        [Fact]
        public void AddCountry_ProperCountry()
        {
            AddCountryRequest req1 = new AddCountryRequest() { CountryName = "USA" };
            CountryResponse res = _countryService.AddCountry(req1);
            Assert.True(res.CountryId != Guid.Empty);
        }
        #endregion
        #region GetCountires
        [Fact]
        public void GetAllCountries_EmptyList()
        {
            List<CountryResponse> actual_country_response_list = _countryService.GetAllCountries();

            Assert.Empty(actual_country_response_list);
        }
        [Fact]
        public void GetAllCountries_Normal()
        {
            List<AddCountryRequest> testList = new List<AddCountryRequest>() { new AddCountryRequest() { CountryName="USA"},
            new AddCountryRequest (){ CountryName="EGY"} };
            List<CountryResponse> ExpectedRes = new List<CountryResponse>();
            foreach (AddCountryRequest test in testList)
            {
                ExpectedRes.Add(_countryService.AddCountry(test));
            }
            List<CountryResponse> actual_country_response = _countryService.GetAllCountries();
            foreach (CountryResponse res in actual_country_response)
            {
                Assert.Contains(res, ExpectedRes);
            }
        }
        #endregion
        #region GetCountryById
        [Fact]
        public void GetCountryById_EmptyId()
        {
            Guid guid = Guid.Empty;
            Assert.Null(_countryService.GetCountryById(guid));
        }
        [Fact]
        public void GetCountryById_NoMatch()
        {
            CountryResponse? res = _countryService.GetCountryById(Guid.NewGuid());
            Assert.Null(res);
        }
        [Fact]
        public void GetCountryById_Normal()
        {
            AddCountryRequest addCountryRequest = new AddCountryRequest() { CountryName = "GER" };
            CountryResponse? Expected = _countryService.AddCountry(addCountryRequest);
            _outputHelper.WriteLine(Expected.CountryId.ToString());
            _outputHelper.WriteLine(_countryService.GetCountryById(Expected.CountryId).CountryId.ToString());
            Assert.Equal(Expected, _countryService.GetCountryById(Expected.CountryId));
        }
        #endregion

    }
}