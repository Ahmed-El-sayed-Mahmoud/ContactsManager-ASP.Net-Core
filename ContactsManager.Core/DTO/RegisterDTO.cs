using ContactsManager.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsManager.Core.DTO
{
	public class RegisterDTO
	{
		[Required(ErrorMessage = "{0} can not be empty")]
		[EmailAddress(ErrorMessage ="{0} should be valid Email")]
        [Remote(action: "IsEmailUsed", controller: "Account", ErrorMessage = "This Email is alraedy used")]
        public string? Email { get; set; }
		[Required(ErrorMessage = "{0} can not be empty")]
		[DataType(DataType.Password)]
		public string? Password { get; set; }
		[Required(ErrorMessage = "{0} can not be empty")]
		[DataType(DataType.Password)]
		[Compare("Password",ErrorMessage ="{0} and {1} should be matched")]
		public string? ConfirmPassword {  get; set; }
		[Required(ErrorMessage = "{0} can not be empty")]

		public string? PersonName { get; set; }
		[Required(ErrorMessage = "{0} can not be empty")]
		[DataType(DataType.PhoneNumber)]
		[Phone]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number should contain numbers only")]
        public string? Phone { get; set;}
		public Roles Role { get; set; } = Roles.User;
	}
}
