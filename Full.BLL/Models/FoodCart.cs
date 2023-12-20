using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Full.BLL.Models
{
	public class FoodCart
	{
		public int FoodId { get; set; }
		public Food Food { get; set; }

		public int Quantity { get; set; } = 1;

		public int CartId { get; set; }
		public Cart Cart { get; set; }
	}
}
