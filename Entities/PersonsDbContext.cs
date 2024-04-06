using Microsoft.Data.SqlClient;
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
            //fluent API
            modelBuilder.Entity<Person>().Property(temp => temp.TIN).HasColumnName("TIN")
                                                                    .HasColumnType("varchar(8)")
                                                                    .HasDefaultValue("ABC87417");
            //modelBuilder.Entity<Person>().HasIndex(temp => temp.TIN).IsUnique();
            modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN", "len([TIN])=8");
        }
        public List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList(); 
        }
        public int sp_AddPerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[] { 
            new SqlParameter("personID",person.PersonID),
            new SqlParameter("personName",person.PersonName),
            new SqlParameter("DateOfBirth",person.DateOfBirth),
            new SqlParameter("Email",person.Email),
            new SqlParameter("Gender",person.Gender),
            new SqlParameter("CountryID",person.CountryID),
            new SqlParameter("Country",person.Country),
            new SqlParameter("Address",person.Address),
            new SqlParameter("ReceiveNewsLetters",person.ReceiveNewsLetters),
            };
           return  Database.ExecuteSqlRaw("EXECUTE [dbo].[AddPerson] @PersonID, @PersonName, @DateOfBirth, @Email, @Gender, @CountryID,@Country, @Address, @ReceiveNewsLetters", parameters);
        }
    }
}
