using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Full.BLL.Models.Credentials;

namespace Full.BLL.Models
{
	public class Order
	{
		public int OrderId { get; set; }
		[Phone, Required]
		public string? PhoneNumber { get; set; }

		[Required]
		public string Address { get; set; }

		[Required]
		public bool PaymentMethod { get; set; }

		[Required]
		public double Total { get; set; }


		[Required, Range(1, 5)]
		public int Status { get; set; } = 1;

		[Required]
		public bool Paid { get; set; }

		[Required]
		public DateTime DateTime { get; set; } = DateTime.UtcNow;
		[Required]
		public double DeliveryFee { get; set; }

		public ApplicationUser User { get; set; }
		public ICollection<FoodOrder> FoodOrders { get; set; }
	}
}
