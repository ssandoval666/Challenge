using Api.Context;
using Api.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.CQRS.Products.Commands;
using Api.CQRS.Products.Queries;


namespace Api.Controllers
{
	[ApiController]
	[Route("api/v{version:apiVersion}/[controller]")]
	[ApiVersion("1.0")]
	[ApiVersion("2.0")] // 👈 añadimos nueva versión

	public class ProductsController : ControllerBase
	{
		private readonly ApiDbContext _context;
		private readonly IMediator _mediator;

		public ProductsController(ApiDbContext context, IMediator mediator)
		{
			_context = context;
			_mediator = mediator;
		}

		#region V1
		
		// GET: api/products
		[HttpGet]
		[MapToApiVersion("1.0")]
		public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
		{
			return await _context.Products
				.Include(p => p.ProductCategories)
					.ThenInclude(pc => pc.Category)
				.ToListAsync();
		}

		[HttpGet("{id}")]
		[MapToApiVersion("1.0")]
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
		[MapToApiVersion("1.0")]
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
		[MapToApiVersion("1.0")]
		public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto dto)
		{
			if (id != dto.ProductID)
				return BadRequest();

			var product = await _context.Products
				.Include(p => p.ProductCategories)
					.ThenInclude(pc => pc.Category)
				.FirstOrDefaultAsync(p => p.ProductID == id);

			if (product == null)
				return NotFound();

			product.Name = dto.Name;
			product.Description = dto.Description;

			
			foreach (var pc in product.ProductCategories)
			{
				_context.ProductCategories.Remove(pc);
			}

			foreach (var catId in dto.CategoryIDs)
			{
				product.ProductCategories.Add(new ProductCategory
				{
					CategoryID = catId
				});
			}


			//_context.Entry(dto).State = EntityState.Modified;

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
		[MapToApiVersion("1.0")]
		public async Task<IActionResult> DeleteProduct(int id)
		{
			var product = await _context.Products.FindAsync(id);
			if (product == null)
				return NotFound();

			_context.Products.Remove(product);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		#endregion

		#region V2

		[HttpGet]
		[MapToApiVersion("2.0")]
		[ApiExplorerSettings(GroupName = "v2")]
		public async Task<IEnumerable<Product>> Get() => await _mediator.Send(new GetProductsQuery());

		[HttpGet("{id:int}")]
		[MapToApiVersion("2.0")]
		[ApiExplorerSettings(GroupName = "v2")]
		public async Task<ActionResult<Product>> GetById(int id)
		{
			var result = await _mediator.Send(new GetProductByIdQuery(id));
			return result is null ? NotFound() : Ok(result);
		}

		[HttpPost]
		[MapToApiVersion("2.0")]
		[ApiExplorerSettings(GroupName = "v2")]
		public async Task<ActionResult<Product>> Create([FromBody] CreateProductCommand command)
		{
			var result = await _mediator.Send(command);
			return CreatedAtAction(nameof(GetById), new { id = result.ProductID }, result);
		}

		[HttpPut("{id}")]
		[MapToApiVersion("2.0")]
		[ApiExplorerSettings(GroupName = "v2")]
		public async Task<ActionResult<Product>> UpdateProduct(int id, [FromBody] UpdateProductCommand command)
		{
			var result = await _mediator.Send(command);
			return CreatedAtAction(nameof(GetById), new { id = result.ProductID }, result);
		}

		[HttpDelete("{id:int}")]
		[MapToApiVersion("2.0")]
		[ApiExplorerSettings(GroupName = "v2")]
		public async Task<IActionResult> Delete(int id)
		{
			var success = await _mediator.Send(new DeleteProductCommand(id));
			return success ? NoContent() : NotFound();
		}

		#endregion
	}

}
