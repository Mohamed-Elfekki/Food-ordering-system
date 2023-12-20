using Full.BLL.Models;
using Full.BLL.Repositories.Interfaces;
using Full.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Full.DAL.Repositories.Classes
{
	public class CartRepository : GenericRepository<Cart>, ICartRepository
	{
		private readonly ApplicationDbContext _context;

		public CartRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}


		public async Task<Cart> IsThereACartAsync(string userId)
		{
			var result = await _context.Carts.Where(c => c.UserId == userId).SingleOrDefaultAsync();
			if (result == null)
			{
				return null;
			}
			return result;
		}
	}
}
