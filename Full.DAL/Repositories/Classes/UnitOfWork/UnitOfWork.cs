using Full.BLL.Dtos.Responses;
using Full.BLL.Repositories.Interfaces;
using Full.BLL.Repositories.Interfaces.UnitOfWork;
using Full.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Full.DAL.Repositories.Classes.UnitOfWork
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly ApplicationDbContext _context;
		public IOrderRepository Orders { get; set; }
		public IFoodRepository Foods { get; set; }
		public ICategoryRepository Categories { get; set; }

		public IFoodCategoryRepository FoodCategories { get; set; }

		public IFoodUserRepository FoodUsers { get; set; }

		public ICartRepository Carts { get; set; }

		public IFoodCartRepository FoodCarts { get; set; }

		public IFoodOrderRepository FoodOrders { get; set; }
		public UnitOfWork(ApplicationDbContext context)
		{
			_context = context;
			Orders = new OrderRepository(_context);
			Foods = new FoodRepository(_context);
			Categories = new CategoryRepository(_context);
			FoodCategories = new FoodCategoryRepository(_context);
			FoodUsers = new FoodUserRepository(_context);
			Carts = new CartRepository(_context);
			FoodCarts = new FoodCartRepository(_context);
			FoodOrders = new FoodOrderRepository(_context);
		}

		public int Commit()
		{
			return _context.SaveChanges();
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
