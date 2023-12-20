using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Full.BLL.Models
{
	public class Category
	{
		public int CategoryId { get; set; }
		[Required, MaxLength(15)]
		public string CategoryType { get; set; }

		[Required]
		public string FilePath { get; set; }
		[Required]
		public string HasedFilePath { get; set; }


		public ICollection<FoodCategory> FoodCategories { get; set; }

	}
}
