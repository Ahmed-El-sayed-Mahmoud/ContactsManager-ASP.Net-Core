using Entities;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountryServices : ICountryServices
    {
        private readonly ICountryRepository _CountryRepository;
        public CountryServices(ICountryRepository countryRepository)
        {
           _CountryRepository = countryRepository;
        }
        public  async Task<CountryResponse> AddCountry(AddCountryRequest request)
        {
            if (request == null) throw new ArgumentNullException("Null Argument");
            if (request.CountryName == null) throw new ArgumentException();
            bool dublicate = await _CountryRepository.GetCountryByCountryName(request.CountryName)!=null;
            if (dublicate)
            {
                throw new ArgumentException("Given Country already exists");
            }
            Country country = request.ToCountry();
            await _CountryRepository.AddCountry(country);
            return country.ToCountryResponse();

        }

        public async Task< List<CountryResponse>> GetAllCountries()
        {
            return (await _CountryRepository.GetAllCountries()).Select(t=>t.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryById(Guid? guid)
        {
            if (guid == null) return null;
            
            return (await _CountryRepository.GetCountryById(guid))?.ToCountryResponse();
        }
    }
}