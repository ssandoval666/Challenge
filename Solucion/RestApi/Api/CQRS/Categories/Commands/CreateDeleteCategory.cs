using Api.Context;
using Api.Models;
using MediatR;

namespace Api.CQRS.Categories.Commands;

public record CreateCategoryCommand(string Name) : IRequest<Category>;
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Category>
{
    private readonly CQRSDbContext _context;
    public CreateCategoryCommandHandler(CQRSDbContext context) => _context = context;

    public async Task<Category> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category { Name = request.Name };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);
        return category;
    }
}

public record DeleteCategoryCommand(int Id) : IRequest<bool>;
public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly CQRSDbContext _context;
    public DeleteCategoryCommandHandler(CQRSDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FindAsync(new object[] { request.Id }, cancellationToken);
        if (category == null) return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
