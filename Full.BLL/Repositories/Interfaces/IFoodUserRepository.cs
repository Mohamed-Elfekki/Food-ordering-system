using Full.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Full.BLL.Repositories.Interfaces
{
	public interface IFoodUserRepository : IGenericRepository<FoodUser>
	{
		Task<IEnumerable<FoodUser>> GetAllFavFoodAsync(string userId);

		Task<FoodUser> GetFavFoodByIdAsync(string userId, int foodId);
	}
}
