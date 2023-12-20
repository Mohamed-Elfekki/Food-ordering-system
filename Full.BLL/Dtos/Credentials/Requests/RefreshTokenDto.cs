using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Full.BLL.Dtos.Credentials.Requests
{
	public class RefreshTokenDto
	{
		[Required]
		public string refreshToken { get; set; }
	}
}
