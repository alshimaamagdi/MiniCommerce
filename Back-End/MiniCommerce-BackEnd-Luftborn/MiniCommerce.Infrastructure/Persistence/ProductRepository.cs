using Microsoft.EntityFrameworkCore;
using MiniCommerce.Application.Common.Interfaces;
using MiniCommerce.Domain.Entities;
using MiniCommerce.Infrastructure.Persistence;

namespace MiniCommerce.Infrastructure.Persistence;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default) =>
        await Context.Products.Where(p => p.CategoryId == categoryId).ToListAsync(cancellationToken);
}
