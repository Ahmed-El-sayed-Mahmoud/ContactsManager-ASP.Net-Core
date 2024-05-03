using Entities.Enums;
using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface IPersonDeleterServices
    {
        Task<bool> DeletePerson(Guid? ID);
       
    }
}
