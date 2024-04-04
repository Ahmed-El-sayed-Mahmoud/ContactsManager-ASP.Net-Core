using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Country
    {
        [StringLength(40)]
        public string? CountryName { get; set; }
        [Key]
        public Guid? CountryID { get; set; }
    }
}