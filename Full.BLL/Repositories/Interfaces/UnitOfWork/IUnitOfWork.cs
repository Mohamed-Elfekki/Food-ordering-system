using Full.BLL.Dtos.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Full.BLL.Repositories.Interfaces.UnitOfWork
{
	public interface IUnitOfWork : IDisposable
	{
		IFoodRepository Foods { get; }
		IOrderRepository Orders { get; }
		ICategoryRepository Categories { get; }

		IFoodCategoryRepository FoodCategories { get; }

		IFoodUserRepository FoodUsers { get; }

		ICartRepository Carts { get; }

		IFoodCartRepository FoodCarts { get; }

		IFoodOrderRepository FoodOrders { get; }

		int Commit();
	}
}
