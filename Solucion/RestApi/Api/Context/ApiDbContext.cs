

using Api.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Api.Context
{
	public class ApiDbContext : DbContext
	{
		public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

		public DbSet<Product> Products { get; set; } = null!;
		public DbSet<Category> Categories { get; set; } = null!;
		public DbSet<ProductCategory> ProductCategories { get; set; } = null!;

		
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Clave compuesta para tabla intermedia
			modelBuilder.Entity<ProductCategory>()
				.HasKey(pc => new { pc.ProductID, pc.CategoryID });

			// Relación muchos a muchos
			modelBuilder.Entity<ProductCategory>()
				.HasOne(pc => pc.Product)
				.WithMany(p => p.ProductCategories)
				.HasForeignKey(pc => pc.ProductID);

			modelBuilder.Entity<ProductCategory>()
				.HasOne(pc => pc.Category)
				.WithMany(c => c.ProductCategories)
				.HasForeignKey(pc => pc.CategoryID);
		}
		
	}

}


