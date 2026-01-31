using MiniCommerce.Application.Common.Interfaces;
using MiniCommerce.Domain.Entities;

namespace MiniCommerce.Application.Categories;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Category
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };
        var created = await _repository.AddAsync(entity, cancellationToken);
        return MapToDto(created);
    }

    public async Task<CategoryDto?> UpdateAsync(Guid id, UpdateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return null;

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(entity, cancellationToken);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;
        await _repository.DeleteAsync(entity, cancellationToken);
        return true;
    }

    private static CategoryDto MapToDto(Category entity) =>
        new(entity.Id, entity.Name, entity.Description, entity.CreatedAt);
}
