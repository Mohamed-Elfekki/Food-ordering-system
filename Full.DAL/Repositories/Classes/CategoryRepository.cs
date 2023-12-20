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
	public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
	{
		private readonly ApplicationDbContext _context;

		public CategoryRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}
	}
}
