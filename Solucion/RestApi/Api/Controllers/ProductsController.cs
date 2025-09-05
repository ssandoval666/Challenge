using Api.Context;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductsController : ControllerBase
	{
		private readonly ApiDbContext _context;

		public ProductsController(ApiDbContext context)
		{
			_context = context;
		}

		// GET: api/products
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
		{
			return await _context.Products
				.Include(p => p.ProductCategories)
					.ThenInclude(pc => pc.Category)
				.ToListAsync();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Product>> GetProduct(int id)
		{
			var product = await _context.Products
				.Include(p => p.ProductCategories)
					.ThenInclude(pc => pc.Category)
				.FirstOrDefaultAsync(p => p.ProductID == id);

			if (product == null) return NotFound();
			return product;
		}

		// POST: api/products
		[HttpPost]
		[HttpPost]
		public async Task<IActionResult> CreateProduct([FromBody] ProductDto dto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var product = new Product
			{
				Name = dto.Name,
				Description = dto.Description,
				//Image = dto.Image
			};

			foreach (var catId in dto.CategoryIDs)
			{
				product.ProductCategories.Add(new ProductCategory
				{
					CategoryID = catId
				});
			}

			_context.Products.Add(product);
			await _context.SaveChangesAsync();

			return Ok(product);
		}

		// PUT: api/products/5
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateProduct(int id, Product product)
		{
			if (id != product.ProductID)
				return BadRequest();

			_context.Entry(product).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!_context.Products.Any(p => p.ProductID == id))
					return NotFound();
				else
					throw;
			}

			return NoContent();
		}

		// DELETE: api/products/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProduct(int id)
		{
			var product = await _context.Products.FindAsync(id);
			if (product == null)
				return NotFound();

			_context.Products.Remove(product);
			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}
