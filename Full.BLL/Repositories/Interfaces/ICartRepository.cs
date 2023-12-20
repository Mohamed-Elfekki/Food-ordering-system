using Full.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Full.BLL.Repositories.Interfaces
{
	public interface ICartRepository : IGenericRepository<Cart>
	{
		Task<Cart> IsThereACartAsync(string userId);

	}
}
