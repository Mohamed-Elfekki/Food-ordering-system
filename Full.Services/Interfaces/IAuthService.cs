using Full.BLL.Dtos.Credentials.Requests;
using Full.BLL.Dtos.Credentials.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Full.Services.Interfaces
{
	public interface IAuthService
	{
		Task<AuthDto> RegisterAsync(RegisterDto dto);
		Task<AuthDto> GetTokenAsync(LoginDto dto);
		Task<string> AddRoleAsync(AddRoleDto dto);
		Task<AuthDto> RefreshTokenAsync(string token);
		Task<bool> RevokeTokenAsync(string token);

		Task<bool> CreateRoleAsync(string roleName);
	}
}
