using Entities;
using Entities.Enums;

namespace ServiceContracts.DTO
{
    public class PersonResponse : IEquatable<PersonResponse>
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public double? Age { get; set; }


        public bool Equals(PersonResponse? other)
        {
            if (ReferenceEquals(other, null)) { return false; }
            return CountryID.Equals(other.CountryID) && PersonName == other.PersonName && Email == other.Email;
        }
        public UpdatePersonRequest ToPersonUpdateRequest()
        {
            return new UpdatePersonRequest()
            {
                PersonId = PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                Address = Address,
                ReceiveNewsLetters = ReceiveNewsLetters,
                CountryId = CountryID
            };
        }
        public override string ToString()
        {
            return $"{PersonName}  {PersonID} {Address}  {Gender} {CountryID}  {Country} {ReceiveNewsLetters} {Email}  {Age} ";
        }
    }
    public static class PersonExtensions
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                Address = person.Address,
                Country = person.Country,
                CountryID = person.CountryID,
                DateOfBirth = person.DateOfBirth,
                Email = person.Email,
                Gender = person.Gender,
                PersonName = person.PersonName,
                PersonID = person.PersonID,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null
            };
        }
    }
}
