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
	public class FoodUserRepository : GenericRepository<FoodUser>, IFoodUserRepository
	{
		private readonly ApplicationDbContext _context;

		public FoodUserRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<IEnumerable<FoodUser>> GetAllFavFoodAsync(string userId)
		{
			var result = await _context.FoodUsers.Include(x => x.Food).Where(x => x.UserId == userId).ToListAsync();
			if (result != null)
			{
				return result;
			}
			return null;
		}

		public async Task<FoodUser> GetFavFoodByIdAsync(string userId, int foodId)
		{
			var result = await _context.FoodUsers.Where(x => x.UserId == userId && x.FoodId == foodId).FirstOrDefaultAsync();
			if (result != null)
			{
				return result;
			}
			return null;
		}


	}
}
