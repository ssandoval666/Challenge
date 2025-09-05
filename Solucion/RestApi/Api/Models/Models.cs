namespace Api.Models
{
	public class Product
	{
		public int ProductID { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }
		public byte[]? Image { get; set; }

		// Navegación muchos a muchos
		public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
	}

	public class Category
	{
		public int CategoryID { get; set; }
		public string Name { get; set; } = string.Empty;

		// Navegación muchos a muchos
		public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
	}

	public class ProductCategory
	{
		public int ProductID { get; set; }
		public Product Product { get; set; } = null!;

		public int CategoryID { get; set; }
		public Category Category { get; set; } = null!;
	}

	public class ProductDto
	{
		public int ProductID { get; set; }
		public string Name { get; set; } = "";
		public string Description { get; set; } = "";
		public string? Image { get; set; }
		public List<int> CategoryIDs { get; set; } = new();
	}

	public class CategoryDto
	{
		public string Name { get; set; } = string.Empty;
	}


}
