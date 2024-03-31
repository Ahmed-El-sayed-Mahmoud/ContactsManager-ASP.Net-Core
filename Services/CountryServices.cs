using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountryServices : ICountryServices
    {
        private readonly List<Country> _countries;
        public CountryServices(bool initialize = true)
        {
            _countries = new List<Country>();
            if (initialize)
            {
                _countries.AddRange(new List<Country>() {
        new Country() {  CountryId = Guid.Parse("000C76EB-62E9-4465-96D1-2C41FDB64C3B"), CountryName = "USA"  },

        new Country() { CountryId = Guid.Parse("32DA506B-3EBA-48A4-BD86-5F93A2E19E3F"), CountryName = "Canada" },

        new Country() { CountryId = Guid.Parse("DF7C89CE-3341-4246-84AE-E01AB7BA476E"), CountryName = "UK" },

        new Country() { CountryId = Guid.Parse("15889048-AF93-412C-B8F3-22103E943A6D"), CountryName = "EGY" },

        new Country() { CountryId = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB"), CountryName = "Australia" }
        });
            }
        }
        public CountryResponse AddCountry(AddCountryRequest request)
        {
            if (request == null) throw new ArgumentNullException("Null Argument");
            if (request.CountryName == null) throw new ArgumentException();
            bool dublicate = _countries.Where(temp => temp.CountryName == request.CountryName).Count() > 0;
            if (dublicate)
            {
                throw new ArgumentException("Given Country already exists");
            }
            Country country = request.ToCountry();
            _countries.Add(country);
            return country.ToCountryResponse();

        }

        public List<CountryResponse> GetAllCountries()
        {
            return _countries.Select(temp => temp.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryById(Guid? guid)
        {
            if (guid == null) return null;
            return _countries.FirstOrDefault(temp => temp.CountryId == guid)?.ToCountryResponse();
        }
    }
}