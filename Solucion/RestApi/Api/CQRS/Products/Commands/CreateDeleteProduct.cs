using Api.Context;
using Api.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.CQRS.Products.Commands;

public record CreateProductCommand(string Name, string? Description, byte[]? Image, List<int> CategoryIDs) : IRequest<Product>;
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product>
{
    private readonly CQRSDbContext _context;
    public CreateProductCommandHandler(CQRSDbContext context) => _context = context;

    public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product { Name = request.Name, Description = request.Description, Image = request.Image };
        
		foreach (var catId in request.CategoryIDs)
		{
			product.ProductCategories.Add(new ProductCategory
			{
				CategoryID = catId
			});
		}

		_context.Products.Add(product);
		await _context.SaveChangesAsync(cancellationToken);

		return product;
    }
}

// ✅ UPDATE
public record UpdateProductCommand(int ProductID, string Name, string? Description, byte[]? Image, List<int> CategoryIDs) : IRequest<Product?>;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Product?>
{
	private readonly CQRSDbContext _context;
	public UpdateProductCommandHandler(CQRSDbContext context) => _context = context;

	public async Task<Product?> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
	{
		var product = await _context.Products
			.Include(p => p.ProductCategories)
			.FirstOrDefaultAsync(p => p.ProductID == request.ProductID, cancellationToken);

		if (product == null) return null;

		// Actualizamos propiedades principales
		product.Name = request.Name;
		product.Description = request.Description;
		product.Image = request.Image;

		// Limpiamos relaciones viejas
		_context.ProductCategories.RemoveRange(product.ProductCategories);

		// Reasignamos categorías
		foreach (var catId in request.CategoryIDs)
		{
			product.ProductCategories.Add(new ProductCategory
			{
				CategoryID = catId,
				ProductID = product.ProductID
			});
		}

		await _context.SaveChangesAsync(cancellationToken);
		return product;
	}
}



public record DeleteProductCommand(int Id) : IRequest<bool>;
public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly CQRSDbContext _context;
    public DeleteProductCommandHandler(CQRSDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
