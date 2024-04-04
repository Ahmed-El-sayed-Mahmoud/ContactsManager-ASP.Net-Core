using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountryServices : ICountryServices
    {
        private readonly PersonsDbContext _db;
        public CountryServices(PersonsDbContext personsDbContext)
        {
           _db = personsDbContext;
        }
        public CountryResponse AddCountry(AddCountryRequest request)
        {
            if (request == null) throw new ArgumentNullException("Null Argument");
            if (request.CountryName == null) throw new ArgumentException();
            bool dublicate = _db.Countries.Where(temp => temp.CountryName == request.CountryName).Count() > 0;
            if (dublicate)
            {
                throw new ArgumentException("Given Country already exists");
            }
            Country country = request.ToCountry();
            _db.Countries.Add(country);
            _db.SaveChanges();
            return country.ToCountryResponse();

        }

        public List<CountryResponse> GetAllCountries()
        {
            return _db.Countries.Select(temp => temp.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryById(Guid? guid)
        {
            if (guid == null) return null;
            return _db.Countries.FirstOrDefault(temp => temp.CountryID == guid)?.ToCountryResponse();
        }
    }
}