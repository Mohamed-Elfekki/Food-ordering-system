using Full.BLL.Dtos.Credentials.Requests;
using Full.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Full.PL.Controllers.Auth
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("Token-Register")]
		public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _authService.RegisterAsync(model);

			if (!result.IsAuthenticated)
				return BadRequest(result.Message);

			//setRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

			return Ok(result);
		}

		[HttpPost("Token-Login")]
		public async Task<IActionResult> GetTokenAsync([FromBody] LoginDto model)
		{
			var result = await _authService.GetTokenAsync(model);

			if (!result.IsAuthenticated)
				return BadRequest(result.Message);

			//if (!string.IsNullOrEmpty(result.RefreshToken))
			//    setRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

			return Ok(result);
		}

		//NEW
		[Authorize(Roles = "Admin"), HttpPost("CreateRole")]
		public async Task<IActionResult> CreateRoleAsync([FromHeader, Required, MaxLength(10)] string roleName)
		{
			var result = await _authService.CreateRoleAsync(roleName);
			if (result == false)
			{
				return BadRequest("Failed To Create New Role");
			}
			return Ok("Role Created Successfully");
		}


		[Authorize(Roles = "Admin"), HttpPost("AddRole")]
		public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleDto model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _authService.AddRoleAsync(model);

			if (!string.IsNullOrEmpty(result))
				return BadRequest(result);

			return Ok(model);
		}

		[HttpPost("RefreshToken")]
		public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
		{
			//var refreshToken = Request.Cookies["refreshToken"];

			var result = await _authService.RefreshTokenAsync(model.refreshToken);

			if (!result.IsAuthenticated)
				return BadRequest(result);

			//setRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

			return Ok(result);
		}

		[HttpPost("RevokeToken")]
		public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenDto model)
		{
			//var token = model.Token ?? Request.Cookies["refreshToken"];
			if (string.IsNullOrEmpty(model.refreshToken))
				return BadRequest("Token is required!");

			var result = await _authService.RevokeTokenAsync(model.refreshToken);

			if (!result)
				return BadRequest("Token is invalid!");

			return Ok();
		}

		private void setRefreshTokenInCookie(string refreshToken, DateTime expires)
		{
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Expires = expires.ToLocalTime(),
				Secure = true,
				IsEssential = true,
				SameSite = SameSiteMode.None
			};

			Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
		}
	}
}
