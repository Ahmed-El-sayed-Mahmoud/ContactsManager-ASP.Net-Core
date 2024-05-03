using Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class Person
    {
        [Key]
        public Guid PersonID { get; set; }
        [StringLength(40)]
        public string? PersonName { get; set; }
        [StringLength(50)]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        [StringLength(40)]
        public string? Country { get; set; }
        [StringLength(200)]
        public string? Address { get; set; }
        public bool? ReceiveNewsLetters { get; set; }
        public string? TIN { get; set; }
        public virtual Country? country { get; set; }
        public override string ToString()
        {
            return $"{PersonID} , {PersonName} , {Country} , {CountryID} , {country?.CountryName}";
        }
    }
    
}
