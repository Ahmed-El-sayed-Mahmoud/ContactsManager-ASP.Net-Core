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
    public class PersonAdderServices : IPersonAdderServices
    {
        private readonly IPersonRepository _personsRepository;
        private readonly ILogger<PersonGetterServices> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonAdderServices(IPersonRepository personRepository,ILogger<PersonGetterServices>logger
            , IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<PersonResponse> AddPerson(AddPersonRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            ValidationHelper.ModelValidator(request);
            Person? dub = (await _personsRepository.GetAllPeople())?.FirstOrDefault(temp => temp.PersonName == request.PersonName!.Trim() && temp.Email == request.Email!.Trim());

            if (dub!=null)
            {
                throw new ArgumentException("This Contact already exist with the same name and Email");
            }
            Person person = request.ToPerson();
           // _personsRepository.Persons.Add(person);
           //await _personsRepository.SaveChangesAsync();
            //_db.sp_AddPerson(person);
            await _personsRepository.AddPerson(person);
            PersonResponse personResponse = person.ToPersonResponse();
            return personResponse;

        }
    }
}
