using Entities;
using Entities.Enums;
using Exceptions;
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
    public class PersonDeleterServices : IPersonDeleterServices
    {
        private readonly IPersonRepository _personsRepository;
        private readonly ILogger<PersonGetterServices> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonDeleterServices(IPersonRepository personRepository,ILogger<PersonGetterServices>logger
            , IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<bool> DeletePerson(Guid? ID)
        {
            if (ID == null) throw new ArgumentNullException();
            if (ID == Guid.Empty) throw new ArgumentNullException(nameof(ID));
            Person? person = await _personsRepository.GetPersonById(ID.Value);
            if (person == null)
            {
                return false;
            }
            await _personsRepository.DeletePerson(ID.Value);
            return true;
        }
    }
}
