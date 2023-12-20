using Full.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Full.BLL.Repositories.Interfaces
{
	public interface IFoodCategoryRepository : IGenericRepository<FoodCategory>
	{

		Task<FoodCategory> GetFoodCategory(int foodId, int categoryId);
	}
}
