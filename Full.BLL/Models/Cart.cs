using Full.BLL.Models.Credentials;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Full.BLL.Models
{
	public class Cart
	{
		public int CartId { get; set; }
		public ICollection<FoodCart> FoodCarts { get; set; }

		public string UserId { get; set; }
		public ApplicationUser User { get; set; }
	}
}
