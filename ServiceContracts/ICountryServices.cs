using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface ICountryServices
    {
        CountryResponse AddCountry(AddCountryRequest request);
        List<CountryResponse> GetAllCountries();
        CountryResponse? GetCountryById(Guid? guid);
    }
}