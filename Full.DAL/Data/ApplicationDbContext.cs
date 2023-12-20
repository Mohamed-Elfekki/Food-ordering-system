using Full.BLL.Models;
using Full.BLL.Models.Credentials;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Full.DAL.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{


		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			base.OnModelCreating(modelBuilder);


			modelBuilder.Entity<FoodCategory>()
				.HasKey(bc => new { bc.FoodId, bc.CategoryId });
			modelBuilder.Entity<FoodCategory>()
				.HasOne(bc => bc.Food)
				.WithMany(b => b.FoodCategories)
				.HasForeignKey(bc => bc.FoodId);
			modelBuilder.Entity<FoodCategory>()
				.HasOne(bc => bc.Category)
				.WithMany(c => c.FoodCategories)
				.HasForeignKey(bc => bc.CategoryId);
			//==========================================================================
			modelBuilder.Entity<FoodCart>()
				.HasKey(bc => new { bc.FoodId, bc.CartId });
			modelBuilder.Entity<FoodCart>()
				.HasOne(bc => bc.Food)
				.WithMany(b => b.FoodCarts)
				.HasForeignKey(bc => bc.FoodId);
			modelBuilder.Entity<FoodCart>()
				.HasOne(bc => bc.Cart)
				.WithMany(c => c.FoodCarts)
				.HasForeignKey(bc => bc.CartId);
			//==========================================================================
			modelBuilder.Entity<FoodUser>()
				.HasKey(bc => new { bc.FoodId, bc.UserId });
			modelBuilder.Entity<FoodUser>()
				.HasOne(bc => bc.Food)
				.WithMany(b => b.FoodUsers)
				.HasForeignKey(bc => bc.FoodId);
			modelBuilder.Entity<FoodUser>()
				.HasOne(bc => bc.User)
				.WithMany(c => c.FoodUsers)
				.HasForeignKey(bc => bc.UserId);
			//==========================================================================
			modelBuilder.Entity<FoodOrder>()
				.HasKey(bc => new { bc.FoodId, bc.OrderId });
			modelBuilder.Entity<FoodOrder>()
				.HasOne(bc => bc.Food)
				.WithMany(b => b.FoodOrders)
				.HasForeignKey(bc => bc.FoodId);
			modelBuilder.Entity<FoodOrder>()
				.HasOne(bc => bc.Order)
				.WithMany(c => c.FoodOrders)
				.HasForeignKey(bc => bc.OrderId);
			//==========================================================================
		}




		public DbSet<Order> Orders { get; set; }
		public DbSet<Cart> Carts { get; set; }
		public DbSet<Food> Foods { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<FoodCategory> FoodCategories { get; set; }
		public DbSet<FoodCart> FoodCarts { get; set; }
		public DbSet<FoodUser> FoodUsers { get; set; }
		public DbSet<FoodOrder> FoodOrders { get; set; }
	}
}
