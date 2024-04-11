using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System.Runtime.InteropServices;

namespace Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly ApplicationDbContext? _db;
        public CountryRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<int> AddCountry(Country country)
        {
            await _db.Countries.AddAsync(country);
            return await _db.SaveChangesAsync();
        }

        public async Task<List<Country>> GetAllCountries()
        {
            return await _db.Countries.ToListAsync();
        }

        public async Task<Country?> GetCountryByCountryName(string countryName)
        {
            return await _db.Countries.FirstOrDefaultAsync(t => t.CountryName == countryName); 
        }

        public async Task<Country?> GetCountryById(Guid? guid)
        {
            return await _db.Countries.FirstOrDefaultAsync(t => t.CountryID == guid);
        }
    }
    
}