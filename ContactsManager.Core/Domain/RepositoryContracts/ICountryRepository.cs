using Entities;

namespace RepositoryContracts
{
    public interface ICountryRepository
    {
        Task<int> AddCountry(Country country);
        Task<List<Country>> GetAllCountries();
        Task<Country?> GetCountryById(Guid? guid);
        Task<Country?> GetCountryByCountryName(string countryName);

    }
}