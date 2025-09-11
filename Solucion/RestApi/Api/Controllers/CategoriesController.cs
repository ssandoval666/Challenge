using Api.Context;
using Api.CQRS.Categories.Commands;
using Api.CQRS.Categories.Queries;
using Api.CQRS.Products.Commands;
using Api.CQRS.Products.Queries;
using Api.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
	[ApiController]
	[Route("api/v{version:apiVersion}/[controller]")]
	[ApiVersion("1.0")]
	[ApiVersion("2.0")] // 👈 añadimos nueva versión
	public class CategoriesController : ControllerBase
	{
		private readonly ApiDbContext _context;
		private readonly IMediator _mediator;
		
		public CategoriesController(ApiDbContext context, IMediator mediator)
		{
			_context = context;
			_mediator = mediator;
		}

		#region v1
		// GET: api/categories
		[HttpGet]
		[MapToApiVersion("1.0")]
		public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
		{
			return await _context.Categories
				.ToListAsync();
		}

		// GET: api/categories/5
		[HttpGet("{id}")]
		[MapToApiVersion("1.0")]
		public async Task<ActionResult<Category>> GetCategory(int id)
		{
			var category = await _context.Categories
				.FirstOrDefaultAsync(c => c.CategoryID == id);

			if (category == null)
				return NotFound();

			return category;
		}

		// POST: api/categories
		[HttpPost]
		[MapToApiVersion("1.0")]
		public async Task<ActionResult<Category>> CreateCategory([FromBody] CategoryDto dto)
		{
			var objCat = new Category();

			objCat.Name = dto.Name;

			_context.Categories.Add(objCat);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetCategory), new { id = objCat.CategoryID }, objCat);
		}

		// PUT: api/categories/5
		[HttpPut("{id}")]
		[MapToApiVersion("1.0")]
		public async Task<IActionResult> UpdateCategory(int id, Category category)
		{
			if (id != category.CategoryID)
				return BadRequest();

			_context.Entry(category).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!_context.Categories.Any(c => c.CategoryID == id))
					return NotFound();
				else
					throw;
			}

			return NoContent();
		}

		// DELETE: api/categories/5
		[HttpDelete("{id}")]
		[MapToApiVersion("1.0")]
		public async Task<IActionResult> DeleteCategory(int id)
		{
			var category = await _context.Categories.FindAsync(id);
			if (category == null)
				return NotFound();

			_context.Categories.Remove(category);
			await _context.SaveChangesAsync();

			return NoContent();
		}
		#endregion


		#region v2
		[HttpGet]
		[MapToApiVersion("2.0")]
		public async Task<IEnumerable<Category>> Get() => await _mediator.Send(new GetCategoriesQuery());

		[HttpGet("{id:int}")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<Category>> GetById(int id)
		{
			var result = await _mediator.Send(new GetCategoryByIdQuery(id));
			return result is null ? NotFound() : Ok(result);
		}

		[HttpPost]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<Category>> Create([FromBody] CreateCategoryCommand command)
		{
			var result = await _mediator.Send(command);
			return CreatedAtAction(nameof(GetById), new { id = result.CategoryID }, result);
		}

		[HttpDelete("{id:int}")]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> Delete(int id)
		{
			var success = await _mediator.Send(new DeleteCategoryCommand(id));
			return success ? NoContent() : NotFound();
		}
		#endregion
	}
}
