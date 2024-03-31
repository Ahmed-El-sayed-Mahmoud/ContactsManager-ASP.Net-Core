using Entities;

namespace ServiceContracts.DTO
{
    public class AddCountryRequest
    {
        public string? CountryName { get; set; }
        public Country ToCountry()
        {
            return new Country() { CountryName = CountryName, CountryId = Guid.NewGuid() };
        }
    }
}
