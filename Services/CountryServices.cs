using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountryServices : ICountryServices
    {
        private readonly ApplicationDbContext _db;
        public CountryServices(ApplicationDbContext personsDbContext)
        {
           _db = personsDbContext;
        }
        public  async Task<CountryResponse> AddCountry(AddCountryRequest request)
        {
            if (request == null) throw new ArgumentNullException("Null Argument");
            if (request.CountryName == null) throw new ArgumentException();
            bool dublicate = await _db.Countries.CountAsync(temp => temp.CountryName == request.CountryName) > 0;
            if (dublicate)
            {
                throw new ArgumentException("Given Country already exists");
            }
            Country country = request.ToCountry();
            await _db.Countries.AddAsync(country);
            await _db.SaveChangesAsync();
            return country.ToCountryResponse();

        }

        public async Task< List<CountryResponse>> GetAllCountries()
        {
            return await _db.Countries.Select(temp => temp.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryById(Guid? guid)
        {
            if (guid == null) return null;
            
            return (await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryID == guid))?.ToCountryResponse();
        }
    }
}