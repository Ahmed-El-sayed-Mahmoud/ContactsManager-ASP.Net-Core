using Entities;

namespace ServiceContracts.DTO
{
    public class CountryResponse : IEquatable<CountryResponse>
    {
        public string? CountryName { get; set; }
        public Guid? CountryId { get; set; }

        public bool Equals(CountryResponse? other)
        {
            if (other == null) return false;
            return other?.CountryId == CountryId;
        }
    }
    public static class CountryExtensions
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse() { CountryName = country.CountryName, CountryId = country.CountryId };
        }
    }
}
