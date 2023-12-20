using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Full.BLL.Dtos.Responses
{
	public class CategoryDtoRes
	{
		public int CategoryId { get; set; }

		public string CategoryType { get; set; }

		public string? FilePath { get; set; }
		public string? HasedFilePath { get; set; }
	}
}
