using Entities.Enums;
using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface IPersonGetterServices
    {
         Task<PersonResponse?> GetPersonById(Guid? ID);
        Task<List<PersonResponse>> GetAllPeople();
         Task<List<PersonResponse>> GetFiltered(string SearchString, string SearchBy);
        Task<MemoryStream> GetPersonsExcel();
    }
}
