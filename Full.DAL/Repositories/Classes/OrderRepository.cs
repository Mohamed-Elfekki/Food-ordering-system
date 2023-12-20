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
	public class OrderRepository : GenericRepository<Order>, IOrderRepository
	{
		private readonly ApplicationDbContext _context;

		public OrderRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Order>> GetAllOrdersDetailDescendingAsync(string userId)
		{
			var result = await _context.Orders.Where(x => x.User.Id == userId).OrderByDescending(x => x.DateTime).ToListAsync();
			if (result.Count > 0)
			{
				return result;
			}
			return null;
		}
	}
}
