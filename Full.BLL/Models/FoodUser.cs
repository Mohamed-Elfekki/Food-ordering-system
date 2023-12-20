using Full.BLL.Models.Credentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Full.BLL.Models
{
	public class FoodUser
	{

		public int FoodId { get; set; }
		[JsonIgnore]
		public Food Food { get; set; }
		public string UserId { get; set; }
		[JsonIgnore]
		public ApplicationUser User { get; set; }
	}
}
