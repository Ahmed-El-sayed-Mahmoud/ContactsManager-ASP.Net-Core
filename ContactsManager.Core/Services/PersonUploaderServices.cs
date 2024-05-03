using Entities;
using Entities.Enums;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContracts;
using Serilog;
using SerilogTimings;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.ValidationHelpers;
using System;
using System.ComponentModel;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace Services
{
    public class PersonUploaderServices : IPersonUploaderServices
    {
        private readonly IPersonRepository _personsRepository;
        private readonly ILogger<PersonGetterServices> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        private readonly IPersonAdderServices _personAdderServices;
        public PersonUploaderServices(IPersonRepository personRepository,ILogger<PersonGetterServices>logger
            , IDiagnosticContext diagnosticContext , IPersonAdderServices personAdderServices)
        {
            _personsRepository = personRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
            _personAdderServices = personAdderServices;
        }
        public async Task<int> UploadExcelFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            int PersonsAdded = 0;
            using(var package=new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["persons"];
                if(worksheet==null)
                {
                    return 0;
                }
                int rowsCount = worksheet.Rows.Count();
                if (rowsCount == 0)
                    return 0;
                
                for(int cur_row = 2; cur_row<=rowsCount; cur_row++)
                {
                    AddPersonRequest person=new AddPersonRequest();
                    if (!string.IsNullOrEmpty(worksheet.Cells[cur_row, 1].Value?.ToString()))
                        person.PersonName = worksheet.Cells[cur_row, 1].Value?.ToString();
                    else
                        continue;
                    
                    if (!string.IsNullOrEmpty(worksheet.Cells[cur_row, 2].Value?.ToString()))
                        person.Email = worksheet.Cells[cur_row, 2].Value?.ToString();
                    else 
                        continue;
                    
                    if (!string.IsNullOrEmpty(worksheet.Cells[cur_row, 3].Value?.ToString()))
                        person.DateOfBirth =Convert.ToDateTime( worksheet.Cells[cur_row, 3].Value);
                   
                    if (!string.IsNullOrEmpty(worksheet.Cells[cur_row, 4].Value?.ToString()))
                        person.Gender = worksheet.Cells[cur_row, 4].Value?.ToString();
                    
                   
                    if (!string.IsNullOrEmpty(worksheet.Cells[cur_row, 6].Value?.ToString()))
                        person.Address = worksheet.Cells[cur_row, 6].Value?.ToString();

                    if (!string.IsNullOrEmpty(worksheet.Cells[cur_row, 7].Value?.ToString()))
                        person.ReceiveNewsLetters = Convert.ToBoolean(worksheet.Cells[cur_row, 7].Value);


                    if(await _personsRepository.GetFiltered(t => t.Email == person.Email && t.PersonName == person.PersonName)==null)
                    {
                        //await AddPerson(person);
                        await _personAdderServices.AddPerson(person);
                        PersonsAdded++;
                    }
                }
            }
            return PersonsAdded;
        }
    }
}
