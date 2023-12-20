using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Full.BLL.Dtos.Requests
{
	public class FoodCartDtoReq
	{
		[Required]
		public int FoodId { get; set; }

		[Required, Range(1, 100)]
		public int Quantity { get; set; }

		[JsonIgnore]
		public int CartId { get; set; }
	}
}
