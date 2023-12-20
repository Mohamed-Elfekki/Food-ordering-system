using Full.BLL.Models;
using Full.BLL.Repositories.Interfaces;
using Full.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Full.DAL.Repositories.Classes
{
	public class FoodOrderRepository : GenericRepository<FoodOrder>, IFoodOrderRepository
	{
		public FoodOrderRepository(ApplicationDbContext context) : base(context)
		{
		}
	}
}
