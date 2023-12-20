using AutoMapper.Configuration.Annotations;
using Full.BLL.Models.Credentials;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Full.BLL.Models
{
	public class Food
	{
		public int FoodId { get; set; }

		[Required, MaxLength(15)]
		public string Title { get; set; }


		[Required, MaxLength(75)]
		public string Description { get; set; }
		[Required]
		public double Price { get; set; }
		[Range(0, 100)]
		public int? Discount { get; set; }
		[Range(0, 5)]
		public int? Rate { get; set; }
		[Required]
		public string FilePath { get; set; }
		[Required]
		public string HasedFilePath { get; set; }




		public ICollection<FoodCategory> FoodCategories { get; set; }
		public ICollection<FoodCart> FoodCarts { get; set; }
		public ICollection<FoodUser> FoodUsers { get; set; }
		public ICollection<FoodOrder> FoodOrders { get; set; }
	}
}
