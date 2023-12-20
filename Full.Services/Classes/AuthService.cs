using Full.BLL.Dtos.Credentials.Requests;
using Full.BLL.Dtos.Credentials.Responses;
using Full.BLL.Models.Credentials;
using Full.BLL.Settings;
using Full.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Full.Services.Classes
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly JWT _jwt;

		public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
			IOptions<JWT> jwt)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_jwt = jwt.Value;
		}

		public async Task<AuthDto> RegisterAsync(RegisterDto model)
		{
			if (await _userManager.FindByEmailAsync(model.Email) is not null)
				return new AuthDto { Message = "Email is already registered!" };

			if (await _userManager.FindByNameAsync(model.Username) is not null)
				return new AuthDto { Message = "Username is already registered!" };

			var user = new ApplicationUser
			{
				UserName = model.Username,
				Email = model.Email,
				FirstName = model.FirstName,
				LastName = model.LastName
			};

			var result = await _userManager.CreateAsync(user, model.Password);

			if (!result.Succeeded)
			{
				var errors = string.Empty;

				foreach (var error in result.Errors)
					errors += $"{error.Description},";

				return new AuthDto { Message = errors };
			}

			await _userManager.AddToRoleAsync(user, "User");

			var jwtSecurityToken = await CreateJwtToken(user);

			var refreshToken = GenerateRefreshToken();
			user.RefreshTokens?.Add(refreshToken);
			await _userManager.UpdateAsync(user);

			return new AuthDto
			{
				Email = user.Email,
				ExpiresOn = jwtSecurityToken.ValidTo,
				IsAuthenticated = true,
				Roles = new List<string> { "User" },
				Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
				Username = user.UserName,
				RefreshToken = refreshToken.Token,
				RefreshTokenExpiration = refreshToken.ExpiresOn
			};
		}

		public async Task<AuthDto> GetTokenAsync(LoginDto model)
		{
			var authDto = new AuthDto();

			var user = await _userManager.FindByEmailAsync(model.Email);

			if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
			{
				authDto.Message = "Email or Password is incorrect!";
				return authDto;
			}

			var jwtSecurityToken = await CreateJwtToken(user);
			var rolesList = await _userManager.GetRolesAsync(user);

			authDto.IsAuthenticated = true;
			authDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
			authDto.Email = user.Email;
			authDto.Username = user.UserName;
			authDto.ExpiresOn = jwtSecurityToken.ValidTo;
			authDto.Roles = rolesList.ToList();

			if (user.RefreshTokens.Any(t => t.IsActive))
			{
				var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
				authDto.RefreshToken = activeRefreshToken.Token;
				authDto.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
			}
			else
			{
				var refreshToken = GenerateRefreshToken();
				authDto.RefreshToken = refreshToken.Token;
				authDto.RefreshTokenExpiration = refreshToken.ExpiresOn;
				user.RefreshTokens.Add(refreshToken);
				await _userManager.UpdateAsync(user);
			}

			return authDto;
		}

		public async Task<string> AddRoleAsync(AddRoleDto model)
		{
			var user = await _userManager.FindByIdAsync(model.UserId);

			if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
				return "Invalid user ID or Role";

			if (await _userManager.IsInRoleAsync(user, model.Role))
				return "User already assigned to this role";

			var result = await _userManager.AddToRoleAsync(user, model.Role);

			return result.Succeeded ? string.Empty : "Something went wrong";
		}

		private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
		{
			var userClaims = await _userManager.GetClaimsAsync(user);
			var roles = await _userManager.GetRolesAsync(user);
			var roleClaims = new List<Claim>();

			foreach (var role in roles)
				roleClaims.Add(new Claim("roles", role));

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim("uid", user.Id)
			}
			.Union(userClaims)
			.Union(roleClaims);

			var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
			var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

			var jwtSecurityToken = new JwtSecurityToken(
				issuer: _jwt.Issuer,
				audience: _jwt.Audience,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
				signingCredentials: signingCredentials);

			return jwtSecurityToken;
		}

		public async Task<AuthDto> RefreshTokenAsync(string token)
		{
			var authModel = new AuthDto();

			var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

			if (user == null)
			{
				authModel.Message = "Invalid token";
				return authModel;
			}

			var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

			if (!refreshToken.IsActive)
			{
				authModel.Message = "Inactive token";
				return authModel;
			}

			refreshToken.RevokedOn = DateTime.UtcNow;

			var newRefreshToken = GenerateRefreshToken();
			user.RefreshTokens.Add(newRefreshToken);
			await _userManager.UpdateAsync(user);

			var jwtToken = await CreateJwtToken(user);
			authModel.IsAuthenticated = true;
			authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
			authModel.Email = user.Email;
			authModel.Username = user.UserName;
			var roles = await _userManager.GetRolesAsync(user);
			authModel.Roles = roles.ToList();
			authModel.RefreshToken = newRefreshToken.Token;
			authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

			return authModel;
		}

		public async Task<bool> RevokeTokenAsync(string token)
		{
			var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

			if (user == null)
				return false;

			var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

			if (!refreshToken.IsActive)
				return false;

			refreshToken.RevokedOn = DateTime.UtcNow;

			await _userManager.UpdateAsync(user);

			return true;
		}

		private RefreshToken GenerateRefreshToken()
		{
			var randomNumber = new byte[32];

			using var generator = new RNGCryptoServiceProvider();

			generator.GetBytes(randomNumber);

			return new RefreshToken
			{
				Token = Convert.ToBase64String(randomNumber),
				ExpiresOn = DateTime.UtcNow.AddMinutes(3),
				CreatedOn = DateTime.UtcNow
			};
		}

		public async Task<bool> CreateRoleAsync(string roleName)
		{
			IdentityRole role = new IdentityRole();
			role.Name = roleName;
			role.ConcurrencyStamp = Guid.NewGuid().ToString();
			var result = await _roleManager.CreateAsync(role);

			if (result != null)
			{
				return true;
			}
			return false;
		}
	}
}
