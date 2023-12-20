using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Full.BLL.Models.Credentials
{
	public class ApplicationUser : IdentityUser
	{
		[Required, MaxLength(15)]
		public string FirstName { get; set; }
		[Required, MaxLength(15)]
		public string LastName { get; set; }

		public ICollection<FoodUser> FoodUsers { get; set; }
		public ICollection<Order> Orders { get; set; }
		public Cart Cart { get; set; }
		public List<RefreshToken>? RefreshTokens { get; set; }
	}
}
