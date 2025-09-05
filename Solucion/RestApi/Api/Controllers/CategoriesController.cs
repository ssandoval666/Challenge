using Api.Context;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CategoriesController : ControllerBase
	{
		private readonly ApiDbContext _context;

		public CategoriesController(ApiDbContext context)
		{
			_context = context;
		}

		// GET: api/categories
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
		{
			return await _context.Categories
				.ToListAsync();
		}

		// GET: api/categories/5
		[HttpGet("{id}")]
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
		public async Task<IActionResult> DeleteCategory(int id)
		{
			var category = await _context.Categories.FindAsync(id);
			if (category == null)
				return NotFound();

			_context.Categories.Remove(category);
			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}
