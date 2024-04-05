using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface ICountryServices
    {
        Task< CountryResponse> AddCountry(AddCountryRequest request);
        Task<List<CountryResponse>> GetAllCountries();
        Task<CountryResponse?> GetCountryById(Guid? guid);
    }
}