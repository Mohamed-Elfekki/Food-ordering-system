using AutoMapper;
using Full.BLL.Dtos.Requests;
using Full.BLL.Dtos.Responses;
using Full.BLL.Models;
using Full.BLL.Repositories.Interfaces.UnitOfWork;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;

namespace Full.PL.Controllers.Admin
{
	[Route("api/[controller]")]
	[ApiController]
	//[Authorize(Roles = "Admin")]
	public class AdminController : ControllerBase
	{
		private new List<string> _allowedExtentions = new List<string> { ".jpg", ".png" };
		private long _maxAllowedPosterSize = 1048576;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IWebHostEnvironment _web;
		bool HasDuplicates(ICollection<int> list)
		{
			var hs = new HashSet<int>();
			foreach (var number in list)
			{
				if (!hs.Add(number))
				{
					return true;
				}
			}
			return false;
		}

		public AdminController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment web)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_web = web;
		}


		#region Food

		[HttpGet("GetAllFoodAsync")]
		public async Task<IActionResult> GetAllFoodAsync()
		{
			var foods = await _unitOfWork.Foods.GetAllFoodAsync();
			if (foods != null)
			{
				var result = _mapper.Map<IEnumerable<FoodDtoRes>>(foods);
				return Ok(result);
			}
			return NotFound("No Food Found");
		}


		[HttpGet("GetFoodByIdAsync")]
		public async Task<IActionResult> GetFoodByIdAsync([FromHeader, Required] int id)
		{
			var food = await _unitOfWork.Foods.GetFoodByIdAsync(id);

			if (food != null)
			{
				var result = _mapper.Map<FoodDtoRes>(food);
				return Ok(result);
			}
			return NotFound($"ID: {id} Not Found");
		}


		[HttpPost("CreateFoodAsync")]
		public async Task<IActionResult> CreateFoodAsync([FromForm] FoodDtoReq dto)
		{
			if (ModelState.IsValid)
			{

				if (!_allowedExtentions.Contains(Path.GetExtension(dto.File.FileName).ToLower()))
				{
					return BadRequest("Only .png, .jpg are Allowed!");
				}
				if (dto.File.Length > _maxAllowedPosterSize)
				{
					return BadRequest("maximum file size is 1 MB !");
				}

				if (dto.CategoryIds is null || dto.CategoryIds.Count == 0)
				{
					return BadRequest(" No CategoryIds added");
				}
				if (HasDuplicates(dto.CategoryIds))
				{
					return BadRequest("The CategoryIDs contain duplicates.");
				}

				foreach (var item in dto.CategoryIds)
				{
					var category = await _unitOfWork.Categories.GetByIdAsync(item);
					if (category == null)
					{
						return NotFound($"CategoryID: {item} Not Found");
					}
				}

				var food = _mapper.Map<Food>(dto);

				if (food != null)
				{
					var root = _web.WebRootPath;
					var path = Path.Combine(root, "Uploads/Foods", food.HasedFilePath);
					using (var fs = System.IO.File.Create(path))
					{
						await dto.File.CopyToAsync(fs);
					}


					var result = await _unitOfWork.Foods.CreateAsync(food);
					if (result != null)
					{
						_unitOfWork.Commit();

						foreach (var item in dto.CategoryIds)
						{
							var foodCategory = new FoodCategory();
							foodCategory.FoodId = result.FoodId;
							foodCategory.CategoryId = item;
							var fC = await _unitOfWork.FoodCategories.CreateAsync(foodCategory);
							if (fC == null)
							{
								return BadRequest($"Item: {result.FoodId} Created Successfully But Failed to add it to CategoryID: {item}");
							}
						}
						_unitOfWork.Commit();
						return Ok("Item Created Successfully");
					}
				}
			}
			return BadRequest(ModelState);
		}

		[HttpPut("UpdateFoodAsync")]
		public async Task<IActionResult> UpdateFoodAsync([FromForm] FoodDtoReq dto, [Required] int foodId)
		{
			if (ModelState.IsValid)
			{
				var isFoodExist = await _unitOfWork.Foods.GetFoodByIdAsync(foodId);
				if (isFoodExist == null)
				{
					return NotFound("foodId Not Found");
				}


				if (!_allowedExtentions.Contains(Path.GetExtension(dto.File.FileName).ToLower()))
				{
					return BadRequest("Only .png, .jpg are Allowed!");
				}
				if (dto.File.Length > _maxAllowedPosterSize)
				{
					return BadRequest("maximum file size is 1 MB !");
				}

				if (dto.CategoryIds is null || dto.CategoryIds.Count == 0)
				{
					return BadRequest(" No CategoryIds added");
				}

				if (HasDuplicates(dto.CategoryIds))
				{
					return BadRequest("The CategoryIDs contain duplicates.");
				}

				foreach (var item in dto.CategoryIds)
				{
					var category = await _unitOfWork.Categories.GetByIdAsync(item);
					if (category == null)
					{
						return NotFound($"CategoryID: {item} Not Found");
					}

					var isFoodCategoryExist = await _unitOfWork.FoodCategories.GetFoodCategory(foodId, item);
					if (isFoodCategoryExist != null)
					{
						return BadRequest($"Item Already Added to CategoryID: {item}");
					}
				}

				var food = _mapper.Map<FoodDtoReq, Food>(dto, isFoodExist);

				if (food != null)
				{
					var root = _web.WebRootPath;
					var path = Path.Combine(root, "Uploads", food.HasedFilePath);
					using (var fs = System.IO.File.Create(path))
					{
						await dto.File.CopyToAsync(fs);
					}
					var result = _unitOfWork.Foods.Update(food);
					if (result is true)
					{
						foreach (var item in dto.CategoryIds)
						{
							var fC = new FoodCategory();
							fC.FoodId = foodId;
							fC.CategoryId = item;
							var f = await _unitOfWork.FoodCategories.CreateAsync(fC);
							if (f == null)
							{
								return BadRequest($"Failed to add food to categoryID:{item}");
							}
						}
						_unitOfWork.Commit();
						return Ok("Item Updated Successfully");
					}
				}
			}
			return BadRequest(ModelState);
		}

		[HttpDelete("DeleteFoodByIdAsync")]
		public async Task<IActionResult> DeleteFoodByIdAsync([FromForm, Required] ICollection<int> foodIds)
		{

			if (foodIds is null || foodIds.Count == 0)
			{
				return BadRequest(" No FoodIds added");
			}

			if (HasDuplicates(foodIds))
			{
				return BadRequest("The FoodIDs contain duplicates");
			}

			foreach (var item in foodIds)
			{
				var food = await _unitOfWork.Foods.GetFoodByIdAsync(item);
				if (food == null)
				{
					return NotFound($"FoodID: {item} Not Found");
				}
				var result = _unitOfWork.Foods.Delete(food);
				if (result == false)
				{
					return BadRequest($"Failed to delete FoodID: {food.FoodId}");
				}
			}
			_unitOfWork.Commit();
			return Ok("Food-Foods Deleted Successfully");
		}

		#endregion


		#region Category

		[HttpGet("GetAllCategoriesAsync")]
		public async Task<IActionResult> GetAllCategoriesAsync()
		{
			var categories = await _unitOfWork.Categories.GetAllAsync();
			if (categories != null)
			{
				var result = _mapper.Map<IEnumerable<CategoryDtoRes>>(categories);
				return Ok(result);
			}
			return NotFound();
		}

		[HttpGet("GetCategoryByIdAsync")]
		public async Task<IActionResult> GetCategoryByIdAsync([FromHeader, Required] int id)
		{
			var category = await _unitOfWork.Categories.GetByIdAsync(id);

			if (category != null)
			{
				var result = _mapper.Map<CategoryDtoRes>(category);
				return Ok(result);
			}
			return NotFound($"ID: {id} Not Found");
		}

		[HttpPost("CreateCategoryAsync")]
		public async Task<IActionResult> CreateCategoryAsync([FromForm] CategoryDtoReq dto)
		{
			if (ModelState.IsValid)
			{

				if (!_allowedExtentions.Contains(Path.GetExtension(dto.File.FileName).ToLower()))
				{
					return BadRequest("Only .png, .jpg are Allowed!");
				}
				if (dto.File.Length > _maxAllowedPosterSize)
				{
					return BadRequest("maximum file size is 1 MB !");
				}

				var category = _mapper.Map<Category>(dto);

				if (category != null)
				{
					var root = _web.WebRootPath;
					var path = Path.Combine(root, "Uploads/Categories", category.HasedFilePath);
					using (var fs = System.IO.File.Create(path))
					{
						await dto.File.CopyToAsync(fs);
					}


					var result = await _unitOfWork.Categories.CreateAsync(category);
					if (result != null)
					{
						_unitOfWork.Commit();
						return Ok("Category Created Successfully");
					}
				}
			}
			return BadRequest(ModelState);
		}

		[HttpPut("UpdateCategoryAsync")]
		public async Task<IActionResult> UpdateCategoryAsync([FromForm] CategoryDtoReq dto, [Required] int categoryId)
		{
			if (ModelState.IsValid)
			{
				var isCategoryExist = await _unitOfWork.Categories.GetByIdAsync(categoryId);
				if (isCategoryExist == null)
				{
					return NotFound("CategoryId Not Found");
				}


				if (!_allowedExtentions.Contains(Path.GetExtension(dto.File.FileName).ToLower()))
				{
					return BadRequest("Only .png, .jpg are Allowed!");
				}
				if (dto.File.Length > _maxAllowedPosterSize)
				{
					return BadRequest("maximum file size is 1 MB !");
				}

				var category = _mapper.Map<CategoryDtoReq, Category>(dto, isCategoryExist);

				if (category != null)
				{
					var root = _web.WebRootPath;
					var path = Path.Combine(root, "Uploads", category.HasedFilePath);
					using (var fs = System.IO.File.Create(path))
					{
						await dto.File.CopyToAsync(fs);
					}

					var result = _unitOfWork.Categories.Update(category);

					if (result is true)
					{
						_unitOfWork.Commit();
						return Ok("Category Updated Successfully");
					}
				}
			}
			return BadRequest(ModelState);
		}

		[HttpDelete("DeleteCategoryByIdAsync")]
		public async Task<IActionResult> DeleteCategoryByIdAsync([FromForm, Required] ICollection<int> categoryIds)
		{
			if (ModelState.IsValid)
			{
				if (categoryIds is null || categoryIds.Count == 0)
				{
					return BadRequest(" No CategoryIds added");
				}

				if (HasDuplicates(categoryIds))
				{
					return BadRequest("The CategoryIDs contain duplicates OR NO Valid Input");
				}

				foreach (var item in categoryIds)
				{
					var category = await _unitOfWork.Categories.GetByIdAsync(item);
					if (category == null)
					{
						return NotFound($"CategoryID: {item} Not Found");
					}
					var result = _unitOfWork.Categories.Delete(category);
					if (result == false)
					{
						return BadRequest($"Failed to delete CategoryID: {category.CategoryId}");
					}
				}
				_unitOfWork.Commit();
				return Ok("Category-Categories Deleted Successfully");
			}
			return BadRequest(ModelState);
		}

		#endregion


		#region Order: "Chef - Delivery"

		[/*Authorize(Roles = "Chef"),*/ HttpGet("GetAllOrdersAsync")]
		public async Task<IActionResult> GetAllOrderAsync()
		{
			var orders = await _unitOfWork.Orders.GetAllAsync();
			if (orders == null)
			{
				return NotFound("No Orders Found");
			}
			var result = _mapper.Map<IEnumerable<OrderDtoRes>>(orders);
			return Ok(result);
		}

		[/*Authorize(Roles = "Delivery"), Authorize(Roles = "Chef"),*/ HttpPut("UpdatingOrderStatusAsync")]
		public async Task<IActionResult> UpdatingOrderStatusAsync([FromHeader, Required] int orderId, [FromHeader, Required, Range(1, 5)] int status)
		{
			var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
			order.Status = status;
			if (order == null)
			{
				return NotFound("Sorry, Order Not Found");
			}
			var x = _unitOfWork.Orders.Update(order);
			if (x == false)
			{
				return BadRequest("Failed to update order");
			}
			_unitOfWork.Commit();
			return Ok("Order updated successfully");
		}
		#endregion


	}

}


