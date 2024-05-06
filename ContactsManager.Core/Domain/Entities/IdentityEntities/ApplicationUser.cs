using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsManager.Core.Domain.Entities.IdentityEntities
{
	public class ApplicationUser:IdentityUser<Guid>
	{
		public string? PersonName { get; set; }
	}
}
