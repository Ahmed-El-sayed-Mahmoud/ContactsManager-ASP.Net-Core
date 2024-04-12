using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts
{
    public interface IPersonRepository
    {
        Task<int> AddPerson(Person person);
        Task<int> DeletePerson(Guid guid);
        Task<int> UpdatePerson(Person person);
        Task<List<Person>> GetAllPeople();
        Task<List<Person>> GetFiltered(Expression<Func<Person,bool>> expression);

        Task<Person?>? GetPersonById(Guid id);

    }
}
