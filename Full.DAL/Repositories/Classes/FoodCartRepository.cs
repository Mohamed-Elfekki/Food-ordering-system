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
	public class FoodCartRepository : GenericRepository<FoodCart>, IFoodCartRepository
	{
		private readonly ApplicationDbContext _context;

		public FoodCartRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<FoodCart> GetFoodCartAsync(int foodId, int cartId)
		{
			var result = await _context.FoodCarts.Where(x => x.FoodId == foodId && x.CartId == cartId).SingleOrDefaultAsync();
			if (result != null)
			{
				return result;
			}
			return null;
		}

		public async Task<IEnumerable<FoodCart>> GetCartDetailsAsync(int cartId)
		{
			var foodCart = await _context.FoodCarts.Include(x => x.Food).Where(x => x.CartId == cartId).ToListAsync();
			if (foodCart != null)
			{
				return foodCart;
			}
			return null;
		}

	}
}
