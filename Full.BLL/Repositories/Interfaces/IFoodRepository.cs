using Full.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Full.BLL.Repositories.Interfaces
{
	public interface IFoodRepository : IGenericRepository<Food>
	{
		Task<IEnumerable<Food>?> GetAllFoodAsync();

		Task<Food?> GetFoodByIdAsync(int id);

		Task<IEnumerable<Food>> GetFilteredFoodAsync(Expression<Func<Food, bool>> filter);
	}
}
