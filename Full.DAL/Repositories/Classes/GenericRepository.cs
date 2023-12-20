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
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private readonly ApplicationDbContext _context;
		private DbSet<T> table;

		public GenericRepository(ApplicationDbContext context)
		{
			_context = context;
			table = _context.Set<T>();
		}




		public async Task<IEnumerable<T>> GetAllAsync()
		{

			var x = await table.ToListAsync();

			if (x.Count == 0)
			{
				return null;
			}
			return x;
		}

		public async Task<T> GetByIdAsync(int id)
		{
			var x = await table.FindAsync(id);

			if (x == null)
			{
				return null;
			}
			return x;
		}

		public async Task<T> CreateAsync(T entity)
		{
			var result = await table.AddAsync(entity);
			if (result != null)
			{
				return entity;
			}
			return null;
		}

		public bool Update(T entity)
		{
			var x = table.Update(entity);
			if (x != null)
			{
				return true;
			}
			return false;
		}

		public bool Delete(T entity)
		{
			var result = table.Remove(entity);
			if (result != null)
			{
				return true;
			}
			return false;
		}

		public void DeleteRange(IEnumerable<T> entities)
		{
			table.RemoveRange(entities);
		}


		public IQueryable<T> GetQueryable()
		{
			throw new NotImplementedException();
		}

	}
}
