using Full.BLL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Full.BLL.Dtos.Requests
{
	public class CategoryDtoReq
	{
		[Required, MaxLength(15)]
		public string CategoryType { get; set; }

		[Required]
		public IFormFile File { get; set; }
	}
}
