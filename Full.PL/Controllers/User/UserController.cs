using AutoMapper;
using Full.BLL.Dtos.Requests;
using Full.BLL.Dtos.Responses;
using Full.BLL.Models;
using Full.BLL.Models.Credentials;
using Full.BLL.Repositories.Interfaces.UnitOfWork;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Full.PL.Controllers.User
{
	[Route("api/[controller]")]
	[ApiController]
	//[Authorize(Roles = "User")]
	public class UserController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly UserManager<ApplicationUser> _userManager;


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

		public UserController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_userManager = userManager;
		}



		#region Foods
		[HttpGet("GetFoodByNameAsync")]
		public async Task<IActionResult> GetFoodByNameAsync([Required] string title)
		{
			var foods = await _unitOfWork.Foods.GetFilteredFoodAsync(food => food.Title.Contains(title));
			if (foods.Count() == 0)
			{
				return NotFound($"No Food Found With Title: {title}");
			}
			var result = _mapper.Map<IEnumerable<FoodDtoRes>>(foods);
			return Ok(result);
		}

		[HttpGet("GetFoodByRateAsync")]
		public async Task<IActionResult> GetFoodByRateAsync([Required, Range(1, 5)] int rate)
		{
			var foods = await _unitOfWork.Foods.GetFilteredFoodAsync(food => food.Rate == rate);
			if (foods.Count() == 0)
			{
				return NotFound($"No Food Found With Rate: {rate}");
			}
			var result = _mapper.Map<IEnumerable<FoodDtoRes>>(foods);
			return Ok(result);
		}

		[HttpGet("GetFoodByPriceAsync")]
		public async Task<IActionResult> GetFoodByPriceAsync(double minPrice, double maxPrice)
		{
			var foods = await _unitOfWork.Foods.GetFilteredFoodAsync(food => food.Price >= minPrice && food.Price <= maxPrice);
			if (foods.Count() == 0)
			{
				return NotFound($"No Food Found With Price: {minPrice}:{maxPrice}");
			}
			var result = _mapper.Map<IEnumerable<FoodDtoRes>>(foods);
			return Ok(result);
		}

		#endregion


		#region Favorite Foods

		[HttpGet("GetAllFavFoodAsync")]
		public async Task<IActionResult> GetAllFavFoodAsync([Required, FromHeader] string userId)
		{

			var isUserExist = await _userManager.FindByIdAsync(userId);
			if (isUserExist == null)
			{
				return NotFound("User Not Found");
			}


			var FavFood = await _unitOfWork.FoodUsers.GetAllFavFoodAsync(userId);
			if (FavFood == null)
			{
				return NotFound("No Fav Food Found");
			}

			var result = _mapper.Map<IEnumerable<FoodDtoRes>>(FavFood);
			return Ok(result);
		}

		[HttpPost("AddFavFoodAsync")]
		public async Task<IActionResult> AddFavFoodAsync([FromHeader, Required] string userId, [FromForm, Required] ICollection<int> foodIds)
		{

			var isUserExist = await _userManager.FindByIdAsync(userId);
			if (isUserExist == null)
			{
				return NotFound("UserID Not Found");
			}

			if (foodIds is null || foodIds.Count == 0)
			{
				return BadRequest("No FoodIds Added");
			}

			if (HasDuplicates(foodIds))
			{
				return BadRequest("The FoodIds contain duplicates OR NO Valid Input");
			}

			foreach (var food in foodIds)
			{
				var isFoodExist = await _unitOfWork.Foods.GetFoodByIdAsync(food);
				if (isFoodExist == null)
				{
					return NotFound($"FoodID: {food} not Found");
				}
				var isFavFoodExist = await _unitOfWork.FoodUsers.GetFavFoodByIdAsync(userId, food);
				if (isFavFoodExist != null)
				{
					return BadRequest($"User Already add this foodID: {food} to Fav");
				}

				var foodUser = new FoodUser();
				foodUser.UserId = userId;
				foodUser.FoodId = food;
				var result = await _unitOfWork.FoodUsers.CreateAsync(foodUser);
				if (result != null)
				{
					continue;
				}
				return BadRequest($"Faild to add FoodID: {food} to favorites");
			}
			_unitOfWork.Commit();
			return Ok("Food - Foods added to favorites successfully");
		}

		[HttpDelete("DeleteFavFoodByIdAsync")]
		public async Task<IActionResult> DeleteFavFoodByIdAsync([FromHeader, Required] string userId, [FromHeader, Required] int foodId)
		{

			var isUserExist = await _userManager.FindByIdAsync(userId);
			if (isUserExist == null)
			{
				return NotFound("UserID Not Found");
			}

			var isFoodExist = await _unitOfWork.Foods.GetFoodByIdAsync(foodId);
			if (isFoodExist == null)
			{
				return NotFound($"FoodID: {foodId} not Found");
			}

			var isFavFoodExist = await _unitOfWork.FoodUsers.GetFavFoodByIdAsync(userId, foodId);
			if (isFavFoodExist == null)
			{
				return BadRequest($"user didn't add this food to his favorites");
			}

			var result = _unitOfWork.FoodUsers.Delete(isFavFoodExist);
			if (result == true)
			{
				_unitOfWork.Commit();
				return Ok("Food removed from favorites Successfully");
			}
			return BadRequest("Faild to delete food from Favorites");

		}

		[HttpDelete("DeleteAllFavFoodAsync")]
		public async Task<IActionResult> DeleteAllFavFoodAsync([FromHeader, Required] string userId)
		{
			var isUserExist = await _userManager.FindByIdAsync(userId);
			if (isUserExist == null)
			{
				return NotFound("UserID Not Found");
			}

			var isAnyFavFoodExist = await _unitOfWork.FoodUsers.GetAllAsync();
			if (isUserExist == null)
			{
				return NotFound("There is NO Food in Favorites");
			}

			_unitOfWork.FoodUsers.DeleteRange(isAnyFavFoodExist);
			_unitOfWork.Commit();
			return Ok("All Favorites Food Deleted successfully");
		}

		#endregion


		#region Carts

		[HttpGet("ShowCartDetailsAsync")]
		public async Task<IActionResult> ShowCartDetailsAsync([FromHeader, Required] string userId)
		{

			var isUserExist = await _userManager.FindByIdAsync(userId);
			if (isUserExist == null)
			{
				return NotFound("User Not Found");
			}

			var isThereACart = await _unitOfWork.Carts.IsThereACartAsync(userId);
			if (isThereACart == null)
			{
				return BadRequest("Sorry, Can't found a Cart for user");
			}

			var foodCart = await _unitOfWork.FoodCarts.GetCartDetailsAsync(isThereACart.CartId);
			var result = _mapper.Map<IEnumerable<FoodCartDtoRes>>(foodCart);
			if (foodCart == null || result == null)
			{
				return BadRequest("Failed to Found your Cart");
			}

			return Ok(result);
		}

		[HttpPost("AddFoodToCartAsync")]
		public async Task<IActionResult> AddFoodToCartAsync([FromHeader, Required] string userId, [FromBody] ICollection<FoodCartDtoReq> dto)
		{

			if (ModelState.IsValid)
			{
				var isUserExist = await _userManager.FindByIdAsync(userId);
				if (isUserExist == null)
				{
					return NotFound("User Not Found");
				}

				if (dto.Count == 0 || dto.IsNullOrEmpty())
				{
					return BadRequest("No FoodIds Added");
				}

				var isThereACart = await _unitOfWork.Carts.IsThereACartAsync(userId);
				if (isThereACart == null)
				{
					isThereACart = new Cart { UserId = userId };
					var newCart = await _unitOfWork.Carts.CreateAsync(isThereACart);
					if (newCart is null)
					{
						return BadRequest("Sorry, Can't found or create a Cart for user");
					}
					_unitOfWork.Commit();
				}


				foreach (var item in dto)
				{
					var isFoodExist = await _unitOfWork.Foods.GetFoodByIdAsync(item.FoodId);
					if (isFoodExist == null)
					{
						return NotFound($"FoodID: {item.FoodId} not Found");
					}

					var isFoodCartExist = await _unitOfWork.FoodCarts.GetFoodCartAsync(item.FoodId, isThereACart.CartId);
					if (isFoodCartExist != null)
					{
						return BadRequest($"foodID: {item.FoodId} already added to your Cart");
					}

					item.CartId = isThereACart.CartId;
					var foodCart = _mapper.Map<FoodCart>(item);

					var addFoodToCartAsync = await _unitOfWork.FoodCarts.CreateAsync(foodCart);
					if (addFoodToCartAsync == null)
					{
						return BadRequest("Faild to add Food to Cart");
					}
				}
				_unitOfWork.Commit();
				return Ok("Food added to your Cart");
			}
			return BadRequest(ModelState);
		}

		[HttpPut("UpdateFoodQuantityAsync")]
		public async Task<IActionResult> UpdateQuantityAsync([FromHeader, Required] string userId, [FromBody, Required] FoodCartDtoReq dto)
		{
			if (ModelState.IsValid)
			{
				var isUserExist = await _userManager.FindByIdAsync(userId);
				if (isUserExist == null)
				{
					return NotFound("User Not Found");
				}

				var isFoodExist = await _unitOfWork.Foods.GetFoodByIdAsync(dto.FoodId);
				if (isFoodExist == null)
				{
					return NotFound($"FoodID: {dto.FoodId} not Found");
				}

				var isThereACart = await _unitOfWork.Carts.IsThereACartAsync(userId);
				if (isThereACart == null)
				{
					return BadRequest("Sorry, Can't found a Cart for user");
				}

				dto.CartId = isThereACart.CartId;
				var foodCart = _mapper.Map<FoodCart>(dto);

				var updateFoodQuantity = _unitOfWork.FoodCarts.Update(foodCart);

				if (updateFoodQuantity is false)
				{
					return BadRequest("Sorry,Can't update Quantity");
				}
				_unitOfWork.Commit();
				return Ok("Quantity Updated Successfully");
			}
			return BadRequest(ModelState);
		}

		[HttpDelete("DeleteFoodFromCartAsync")]
		public async Task<IActionResult> DeleteFoodFromCartAsync([FromHeader, Required] string userId, [FromForm, Required] ICollection<int> foodIds)
		{

			if (ModelState.IsValid)
			{
				var isUserExist = await _userManager.FindByIdAsync(userId);
				if (isUserExist == null)
				{
					return NotFound("UserID Not Found");
				}

				if (foodIds is null || foodIds.Count == 0)
				{
					return BadRequest(" No FoodIDs added");
				}

				var isThereACart = await _unitOfWork.Carts.IsThereACartAsync(userId);
				if (isThereACart == null)
				{
					isThereACart = new Cart { UserId = userId };
					var newCart = await _unitOfWork.Carts.CreateAsync(isThereACart);
					if (newCart is null)
					{
						return BadRequest("Sorry, Can't found or create a Cart for user");
					}
					_unitOfWork.Commit();
				}


				if (HasDuplicates(foodIds))
				{
					return BadRequest("The FoodIds contain duplicates OR NO Valid Input");
				}

				foreach (var item in foodIds)
				{

					var food = await _unitOfWork.Foods.GetByIdAsync(item);
					if (food == null)
					{
						return NotFound($"FoodId: {item} Not Found");
					}

					var isFoodCartExist = await _unitOfWork.FoodCarts.GetFoodCartAsync(item, isThereACart.CartId);
					if (isFoodCartExist == null)
					{
						return BadRequest($"foodID: {item} NOT exist in your Cart");
					}

					var result = _unitOfWork.FoodCarts.Delete(isFoodCartExist);
					if (result == false)
					{
						return BadRequest($"Failed to delete FoodId: {item} from Cart");
					}
				}
				_unitOfWork.Commit();
				return Ok("Food - Foods Deleted Successfully from your Cart");
			}
			return BadRequest(ModelState);

		}

		[HttpDelete("EmptyCartAsync")]
		public async Task<IActionResult> EmptyCartAsync([FromHeader, Required] string userId)
		{
			var isUserExist = await _userManager.FindByIdAsync(userId);
			if (isUserExist == null)
			{
				return NotFound("UserID Not Found");
			}

			var isThereACart = await _unitOfWork.Carts.IsThereACartAsync(userId);
			if (isThereACart == null)
			{
				return NotFound("There is NO Cart");
			}

			var result = _unitOfWork.Carts.Delete(isThereACart);
			if (result == false)
			{
				return BadRequest("Failed to empty the cart");
			}
			_unitOfWork.Commit();
			return Ok("The Cart is Empty");

		}

		#endregion


		#region Orders

		[HttpPost("ConfirmOrderAsync")]
		public async Task<IActionResult> ConfirmOrderAsync([FromBody, Required] string userId, [FromForm] OrderDtoReq dto)
		{
			if (ModelState.IsValid)
			{

				var isUserExist = await _userManager.FindByIdAsync(userId);
				if (isUserExist == null)
				{
					return NotFound("UserID Not Found");
				}

				var isThereACart = await _unitOfWork.Carts.IsThereACartAsync(userId);
				if (isThereACart == null)
				{
					return NotFound("There is NO Cart");
				}

				var order = _mapper.Map<Order>(dto);
				var foodCart = await _unitOfWork.FoodCarts.GetCartDetailsAsync(isThereACart.CartId);

				foreach (var item in foodCart)
				{
					order.Total += item.Food.Price;
				}
				//order.PaymentMethod = false ? order.Paid = true : order.Paid = false;

				if (order.PaymentMethod == false) { order.Paid = true; }
				else { order.Paid = false; }

				if (order.Total >= 1000) { order.DeliveryFee = 0; }
				else { order.DeliveryFee = (2 * order.Total) / 100; }


				order.User = isUserExist;
				var result = await _unitOfWork.Orders.CreateAsync(order);

				if (result == null)
				{
					return BadRequest("Failed to Confirm Order");
				}

				_unitOfWork.Commit();

				foreach (var item in foodCart)
				{
					FoodOrder foodOrder = new FoodOrder();
					foodOrder.FoodId = item.FoodId;
					foodOrder.OrderId = result.OrderId;
					foodOrder.Quantity = item.Quantity;
					await _unitOfWork.FoodOrders.CreateAsync(foodOrder);
				}

				_unitOfWork.Commit();
				return Ok("Order Confirmed Successfully");
			}
			return BadRequest(ModelState);
		}

		[HttpGet("GetAllOrdersDetailDescendingAsync")]
		public async Task<IActionResult> GetAllOrdersDetailDescendingAsync([FromHeader, Required] string userId)
		{
			var isUserExist = await _userManager.FindByIdAsync(userId);
			if (isUserExist == null)
			{
				return NotFound("UserID Not Found");
			}
			var isOrdersExist = await _unitOfWork.Orders.GetAllOrdersDetailDescendingAsync(userId);
			if (isOrdersExist == null)
			{
				return NotFound("Sorry, Orders Not Found");
			}
			var orders = _mapper.Map<IEnumerable<OrderDtoRes>>(isOrdersExist);
			return Ok(orders);

		}

		[HttpGet("GetOrderDetailAsync")]
		public async Task<IActionResult> GetOrderDetailAsync([FromHeader, Required] string userId, [FromHeader, Required] int orderId)
		{
			var isUserExist = await _userManager.FindByIdAsync(userId);
			if (isUserExist == null)
			{
				return NotFound("UserID Not Found");
			}
			var result = await _unitOfWork.Orders.GetByIdAsync(orderId);
			if (result == null)
			{
				return NotFound("Order Not Found");
			}

			var order = _mapper.Map<OrderDtoRes>(result);
			return Ok(order);
		}

		[HttpDelete("CancelOrderAsync")]
		public async Task<IActionResult> CancelOrderAsync([FromHeader, Required] string userId, [FromHeader, Required] int orderId)
		{
			var isUserExist = await _userManager.FindByIdAsync(userId);
			if (isUserExist == null)
			{
				return NotFound("UserID Not Found");
			}
			var result = await _unitOfWork.Orders.GetByIdAsync(orderId);
			if (result == null)
			{
				return NotFound("Order Not Found");
			}

			var deletedOrder = _unitOfWork.Orders.Delete(result);

			if (deletedOrder == false)
			{
				return BadRequest("Sorry, Can't Cancel Your Order");
			}
			_unitOfWork.Commit();
			return Ok("Your Order Canceled Successfully");

		}

		#endregion


	}
}
