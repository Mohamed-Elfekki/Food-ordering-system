using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Full.BLL.Dtos.Responses
{
	public class OrderDtoRes
	{
		public int OrderId { get; set; }

		public string? PhoneNumber { get; set; }


		public string Address { get; set; }


		public bool PaymentMethod { get; set; }


		public double Total { get; set; }


		public int Status { get; set; }


		public bool Paid { get; set; }

		public DateTime DateTime { get; set; }

		public double DeliveryFee { get; set; }
	}
}
