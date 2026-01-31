using MiniCommerce.Application.Common.Interfaces;
using MiniCommerce.Domain.Entities;
using MiniCommerce.Infrastructure.Persistence;

namespace MiniCommerce.Infrastructure.Persistence;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context) { }
}
