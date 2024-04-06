using Entities.Enums;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface IPersonServices
    {
        Task<PersonResponse> AddPerson(AddPersonRequest request);
         Task<PersonResponse?> GetPersonById(Guid? ID);
        Task<List<PersonResponse>> GetAllPeople();
         Task<List<PersonResponse>> GetFiltered(string SearchString, string SearchBy);
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);
        Task<PersonResponse> UpdatePerson(UpdatePersonRequest? personUpdateRequest);
        Task<bool> DeletePerson(Guid? ID);
        Task<MemoryStream> GetPersonsExcel();
    }
}
