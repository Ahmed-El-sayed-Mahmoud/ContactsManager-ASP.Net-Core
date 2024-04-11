using Entities;
using Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    public class AddPersonRequest
    {
        [Required(ErrorMessage = "Person Name can't be blank")]
        public string? PersonName { get; set; }

        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email value should be a valid email")]
        public string? Email { get; set; }

        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "Choose a Gender")]
        public string? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; } 
        public bool ReceiveNewsLetters { get; set; }
        public Person ToPerson()
        {
            return new Person()
            {
                Address = Address,
                CountryID = CountryId,
                DateOfBirth = DateOfBirth,
                Email = Email?.Trim(),
                Gender =Gender,
                PersonName = PersonName?.Trim(),
                PersonID = Guid.NewGuid(),
                ReceiveNewsLetters = ReceiveNewsLetters,
                Country=Country,

            };
        }
    }
}
