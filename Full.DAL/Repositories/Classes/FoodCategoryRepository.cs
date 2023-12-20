using Full.BLL.Models;
using Full.BLL.Repositories.Interfaces;
using Full.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Full.DAL.Repositories.Classes
{
	public class FoodCategoryRepository : GenericRepository<FoodCategory>, IFoodCategoryRepository
	{
		private readonly ApplicationDbContext _context;

		public FoodCategoryRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<FoodCategory> GetFoodCategory(int foodId, int categoryId)
		{
			var result = await _context.FoodCategories.Where(x => x.FoodId == foodId && x.CategoryId == categoryId).SingleOrDefaultAsync();
			if (result != null)
			{
				return result;
			}
			return null;

		}
	}
}
