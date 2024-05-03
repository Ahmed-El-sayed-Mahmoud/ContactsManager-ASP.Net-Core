using Entities.Enums;
using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface IPersonAdderServices
    {
        Task<PersonResponse> AddPerson(AddPersonRequest request);
    }
}
