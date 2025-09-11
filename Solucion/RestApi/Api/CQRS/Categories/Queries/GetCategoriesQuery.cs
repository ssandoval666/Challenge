using Api.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.CQRS.Categories.Queries;
public record GetCategoriesQuery() : IRequest<IEnumerable<Category>>;
public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IEnumerable<Category>>
{
    private readonly CQRSDbContext _context;
    public GetCategoriesQueryHandler(CQRSDbContext context) => _context = context;

    public async Task<IEnumerable<Category>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        => await _context.Categories.AsNoTracking().ToListAsync(cancellationToken);
}

public record GetCategoryByIdQuery(int Id) : IRequest<Category?>;
public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Category?>
{
    private readonly CQRSDbContext _context;
    public GetCategoryByIdQueryHandler(CQRSDbContext context) => _context = context;

    public async Task<Category?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        => await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryID == request.Id, cancellationToken);
}
