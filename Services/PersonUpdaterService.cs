using Entities;
using Entities.Enums;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
    public class PersonUpdaterServices : IPersonUpdaterServices
    {
        private readonly IPersonRepository _personsRepository;
        private readonly ILogger<PersonGetterServices> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonUpdaterServices(IPersonRepository personRepository,ILogger<PersonGetterServices>logger
            , IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<PersonResponse> UpdatePerson(UpdatePersonRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(personUpdateRequest));
            ValidationHelper.ModelValidator(personUpdateRequest);
            Person? person = await _personsRepository.GetPersonById(personUpdateRequest.PersonId);
            if (person == null)
                throw new InvalidIDException("This ID does not exist in your Contacts");
            person.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            person.Gender = personUpdateRequest.Gender;
            person.Email = personUpdateRequest.Email;
            person.DateOfBirth = personUpdateRequest.DateOfBirth;
            person.Address = personUpdateRequest.Address;
            person.CountryID = personUpdateRequest.CountryId;
           await  _personsRepository.UpdatePerson(person);

            return person.ToPersonResponse();

        }
    }
}
