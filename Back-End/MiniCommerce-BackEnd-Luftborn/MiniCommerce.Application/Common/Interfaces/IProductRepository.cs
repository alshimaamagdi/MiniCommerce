using MiniCommerce.Domain.Entities;

namespace MiniCommerce.Application.Common.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
}
