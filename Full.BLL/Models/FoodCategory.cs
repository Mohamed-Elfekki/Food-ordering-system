using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Full.BLL.Models
{
	public class FoodCategory
	{
		[JsonIgnore]
		public int FoodId { get; set; }
		[JsonIgnore]
		public Food Food { get; set; }

		public int CategoryId { get; set; }
		[JsonIgnore]
		public Category Category { get; set; }
	}
}
