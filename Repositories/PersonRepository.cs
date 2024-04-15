using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext? _db;
        private readonly ILogger<PersonRepository> _logger;
        public PersonRepository(ApplicationDbContext db, ILogger<PersonRepository> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<int> AddPerson(Person person)
        {
             await _db.Persons.AddAsync(person);
            return await _db.SaveChangesAsync();

        }

        public async Task<int> DeletePerson(Guid guid)
        {
            _db.Persons.Remove(_db.Persons.FirstOrDefault(t => t.PersonID == guid));
            return await _db.SaveChangesAsync();
        }

        public async Task<List<Person>> GetAllPeople()
        {
            _logger.LogDebug("Get All People is reached");
            return await _db.Persons.Include("country").ToListAsync();
        }

        public async Task<List<Person>> GetFiltered(Expression<Func<Person, bool>>predicate)
        {
            return await _db.Persons.Where(predicate).ToListAsync();
        }

        public async Task<Person?>? GetPersonById(Guid id)
        {
            return await _db.Persons.FirstOrDefaultAsync(t => t.PersonID == id);
        }

        public async Task<int> UpdatePerson(Person person)
        {
            Person? matching= await _db.Persons.FirstOrDefaultAsync(t=>t.PersonID==person.PersonID);
            if(matching!=null) { return 0; }
            matching.Gender = person.Gender;
            matching.Address = person.Address;
            matching.Country = person.Country;
            matching.Email=person.Email;
            matching.PersonName= person.PersonName;
            return await _db.SaveChangesAsync();
        }
    }
}
