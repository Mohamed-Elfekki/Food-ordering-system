using AutoMapper;
using AutoMapper.Internal;
using Full.BLL.Dtos.Requests;
using Full.BLL.Dtos.Responses;
using Full.BLL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Full.BLL.Mappers
{
	public class MappingProfile : Profile
	{

		public MappingProfile()
		{
			CreateMap<Food, FoodDtoRes>()
				.ForMember(dest => dest.PriceAfterDiscount, opt => opt.MapFrom(src => (src.Price - (src.Discount * src.Price) / 100)));


			CreateMap<Food, FoodCategory>()
				.ForMember(dest => dest.Category, src => src.Ignore())
				.ForMember(dest => dest.Food, src => src.Ignore());

			var newName = Guid.NewGuid().ToString();
			CreateMap<FoodDtoReq, Food>()
				.ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.File.FileName))
				.ForMember(dest => dest.HasedFilePath, opt => opt.MapFrom(src => string.Concat(newName, Path.GetExtension(src.File.FileName))));


			CreateMap<Category, CategoryDtoRes>();
			CreateMap<CategoryDtoReq, Category>()
				.ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.File.FileName))
				.ForMember(dest => dest.HasedFilePath, opt => opt.MapFrom(src => string.Concat(newName, Path.GetExtension(src.File.FileName))));


			CreateMap<FoodUser, FoodDtoRes>()
			.ForMember(dest => dest.FoodId, opt => opt.MapFrom(src => src.Food.FoodId))
			.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Food.Title))
			.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Food.Description))
			.ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Food.Price))
			.ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Food.Discount))
			.ForMember(dest => dest.PriceAfterDiscount, opt => opt.MapFrom(src => (src.Food.Price - (src.Food.Discount * src.Food.Price) / 100)))
			.ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Food.Rate))
			.ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.Food.FilePath))
			.ForMember(dest => dest.HasedFilePath, opt => opt.MapFrom(src => src.Food.HasedFilePath))
			.ForMember(dest => dest.FoodCategories, opt => opt.MapFrom(src => src.Food.FoodCategories));


			CreateMap<FoodCart, FoodCartDtoRes>()
			.ForMember(dest => dest.FoodId, opt => opt.MapFrom(src => src.Food.FoodId))
			.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Food.Title))
			.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Food.Description))
			.ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Food.Price))
			.ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Food.Discount))
			.ForMember(dest => dest.PriceAfterDiscount, opt => opt.MapFrom(src => (src.Food.Price - (src.Food.Discount * src.Food.Price) / 100)))
			.ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Food.Rate))
			.ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.Food.FilePath))
			.ForMember(dest => dest.HasedFilePath, opt => opt.MapFrom(src => src.Food.HasedFilePath))
			.ForMember(dest => dest.FoodCategories, opt => opt.MapFrom(src => src.Food.FoodCategories));

			CreateMap<FoodCartDtoReq, FoodCart>();

			CreateMap<OrderDtoReq, Order>()
				.ForMember(dest => dest.Paid, opt => opt.MapFrom(src => src.PaymentMethod == false ? true : false));

			CreateMap<Order, OrderDtoRes>();
		}
	}
}
