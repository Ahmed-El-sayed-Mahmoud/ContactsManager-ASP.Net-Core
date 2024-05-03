using Entities.Enums;
using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface IPersonUpdaterServices
    {
      
        Task<PersonResponse> UpdatePerson(UpdatePersonRequest? personUpdateRequest);
    }
}
