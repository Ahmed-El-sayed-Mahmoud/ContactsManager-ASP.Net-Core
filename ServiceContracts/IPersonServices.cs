using Entities.Enums;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface IPersonServices
    {
        PersonResponse AddPerson(AddPersonRequest request);
        PersonResponse? GetPersonById(Guid? ID);
        List<PersonResponse> GetAllPeople();
        List<PersonResponse> GetFiltered(string SearchString, string SearchBy);
        List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);
        PersonResponse UpdatePerson(UpdatePersonRequest? personUpdateRequest);
        bool DeletePerson(Guid? ID);
    }
}
