using Full.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Full.BLL.Dtos.Responses
{
	public class FoodCartDtoRes : FoodDtoRes
	{
		//public int FoodId { get; set; }
		//public string Title { get; set; }


		//public string Description { get; set; }


		//public double Price { get; set; }

		//public int? Discount { get; set; }

		//public double PriceAfterDiscount { get; set; }

		//public int? Rate { get; set; }
		//public string? FilePath { get; set; }
		//public string? HasedFilePath { get; set; }
		//public ICollection<FoodCategory> FoodCategories { get; set; }
		public int Quantity { get; set; }
	}
}
