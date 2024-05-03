using Entities.Enums;
using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface IPersonUploaderServices
    {
        Task<int> UploadExcelFile(IFormFile formFile);
    }
}
