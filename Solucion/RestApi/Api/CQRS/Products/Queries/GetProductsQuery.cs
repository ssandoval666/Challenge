using Api.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Api.Models;


namespace Api.CQRS.Products.Queries;
public record GetProductsQuery() : IRequest<IEnumerable<Product>>;
public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IEnumerable<Product>>
{
    private readonly CQRSDbContext _context;
    public GetProductsQueryHandler(CQRSDbContext context) => _context = context;

    public async Task<IEnumerable<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        => await _context.Products
				.Include(p => p.ProductCategories)
					.ThenInclude(pc => pc.Category)
        .AsNoTracking().ToListAsync(cancellationToken);
}

public record GetProductByIdQuery(int Id) : IRequest<Product?>;
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product?>
{
    private readonly CQRSDbContext _context;
    public GetProductByIdQueryHandler(CQRSDbContext context) => _context = context;

    public async Task<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        => await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductID == request.Id, cancellationToken);
}
