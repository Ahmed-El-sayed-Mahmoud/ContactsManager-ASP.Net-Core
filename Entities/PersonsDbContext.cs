using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Entities
{
    public class PersonsDbContext:DbContext
    {
        public DbSet<Person>? Persons { get; set; }
        public DbSet<Country>?Countries { get; set; }

        public PersonsDbContext(DbContextOptions<PersonsDbContext> options) : base(options) { 
        
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");
            string CountriesString = System.IO.File.ReadAllText("countries.json");
            List<Country>?CountriesList= System.Text.Json.JsonSerializer.Deserialize<List<Country>>(CountriesString);
            foreach (Country country in CountriesList)
                modelBuilder.Entity<Country>().HasData(country);
            string PersonsString = System.IO.File.ReadAllText("persons.json");
            List<Person>? PersonsList = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(PersonsString);
            foreach (Person person in PersonsList)
                modelBuilder.Entity<Person>().HasData(person);
        }
        public List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList(); 
        }
    }
}
