using MiniCommerce.Application.Common.Interfaces;
using MiniCommerce.Domain.Entities;

namespace MiniCommerce.Application.Products;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _productRepository.GetAllAsync(cancellationToken);
        return await MapToDtosAsync(entities, cancellationToken);
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return null;
        var list = await MapToDtosAsync(new[] { entity }, cancellationToken);
        return list[0];
    }

    public async Task<IReadOnlyList<ProductDto>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var entities = await _productRepository.GetByCategoryIdAsync(categoryId, cancellationToken);
        return await MapToDtosAsync(entities, cancellationToken);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            CategoryId = dto.CategoryId,
            CreatedAt = DateTime.UtcNow
        };
        var created = await _productRepository.AddAsync(entity, cancellationToken);
        var list = await MapToDtosAsync(new[] { created }, cancellationToken);
        return list[0];
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return null;

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Price = dto.Price;
        entity.StockQuantity = dto.StockQuantity;
        entity.CategoryId = dto.CategoryId;
        entity.UpdatedAt = DateTime.UtcNow;
        await _productRepository.UpdateAsync(entity, cancellationToken);
        var list = await MapToDtosAsync(new[] { entity }, cancellationToken);
        return list[0];
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;
        await _productRepository.DeleteAsync(entity, cancellationToken);
        return true;
    }

    private async Task<IReadOnlyList<ProductDto>> MapToDtosAsync(IEnumerable<Product> entities, CancellationToken cancellationToken)
    {
        var list = entities.ToList();
        var categoryIds = list.Select(p => p.CategoryId).Distinct().ToList();
        var categories = new Dictionary<Guid, string>();
        foreach (var cid in categoryIds)
        {
            var cat = await _categoryRepository.GetByIdAsync(cid, cancellationToken);
            if (cat is not null) categories[cid] = cat.Name;
        }
        return list.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.StockQuantity,
            p.CategoryId,
            categories.GetValueOrDefault(p.CategoryId),
            p.CreatedAt)).ToList();
    }
}
