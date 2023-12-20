using Full.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Full.BLL.Repositories.Interfaces
{
	public interface IFoodCartRepository : IGenericRepository<FoodCart>
	{
		Task<FoodCart> GetFoodCartAsync(int foodId, int cartId);

		Task<IEnumerable<FoodCart>> GetCartDetailsAsync(int cartId);
	}
}
