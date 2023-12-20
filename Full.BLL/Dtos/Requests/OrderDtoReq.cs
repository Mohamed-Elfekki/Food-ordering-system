using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Full.BLL.Dtos.Requests
{
	public class OrderDtoReq
	{
		[Phone, Required]
		public string? PhoneNumber { get; set; }

		[Required]
		public string Address { get; set; }

		[Required]
		public bool PaymentMethod { get; set; }
	}
}
