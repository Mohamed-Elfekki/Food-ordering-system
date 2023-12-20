using Full.BLL.Models;
using Full.BLL.Repositories.Interfaces;
using Full.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Full.DAL.Repositories.Classes
{
	public class FoodRepository : GenericRepository<Food>, IFoodRepository
	{
		private readonly ApplicationDbContext _context;
		public FoodRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}


		public async Task<IEnumerable<Food>?> GetAllFoodAsync()
		{
			var result = await _context.Foods.Include(x => x.FoodCategories).ToListAsync();
			if (result.Count > 0)
			{
				return result;
			}
			return null;
		}
		public async Task<IEnumerable<Food>> GetFilteredFoodAsync(Expression<Func<Food, bool>> filter)
		{
			return await _context.Foods.Where(filter).ToListAsync();
		}

		public async Task<Food?> GetFoodByIdAsync(int id)
		{
			var result = await _context.Foods.Include(x => x.FoodCategories).Where(x => x.FoodId == id).SingleOrDefaultAsync();
			if (result != null)
			{
				return result;
			}
			return null;
		}
	}
}
