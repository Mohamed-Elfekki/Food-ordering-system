using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Full.BLL.Repositories.Interfaces
{
	public interface IGenericRepository<T> where T : class
	{
		Task<IEnumerable<T>> GetAllAsync();

		Task<T> GetByIdAsync(int id);

		Task<T> CreateAsync(T entity);

		bool Update(T entity);
		bool Delete(T entity);

		void DeleteRange(IEnumerable<T> entities);

		IQueryable<T> GetQueryable();


	}
}
