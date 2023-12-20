using Full.BLL.Models.Credentials;
using Full.BLL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Full.BLL.Dtos.Requests
{
	public class FoodDtoReq
	{

		[Required, MaxLength(15)]
		public string Title { get; set; }

		[Required, MaxLength(75)]
		public string Description { get; set; }

		[Required]
		public double Price { get; set; }

		[Range(0, 100)]
		public int? Discount { get; set; }

		[Range(1, 5)]
		public int? Rate { get; set; }

		[Required]
		public IFormFile File { get; set; }

		//public int CategoryId { get; set; }
		//[Required]
		public ICollection<int>? CategoryIds { get; set; }

	}
}
