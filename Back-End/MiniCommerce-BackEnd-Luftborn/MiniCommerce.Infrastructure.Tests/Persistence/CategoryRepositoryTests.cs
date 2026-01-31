using Microsoft.EntityFrameworkCore;
using MiniCommerce.Domain.Entities;
using MiniCommerce.Infrastructure.Persistence;
using Xunit;

namespace MiniCommerce.Infrastructure.Tests.Persistence;

public class CategoryRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CategoryRepository _sut;

    public CategoryRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "CategoryRepo_" + Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _sut = new CategoryRepository(_context);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task AddAsync_Adds_Category_To_Db()
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Electronics",
            Description = "Devices",
            CreatedAt = DateTime.UtcNow
        };

        var result = await _sut.AddAsync(category);

        Assert.Same(category, result);
        var fromDb = await _context.Categories.FindAsync(category.Id);
        Assert.NotNull(fromDb);
        Assert.Equal("Electronics", fromDb.Name);
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Null_When_Not_Found()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Category_When_Found()
    {
        var id = Guid.NewGuid();
        var category = new Category { Id = id, Name = "Books", CreatedAt = DateTime.UtcNow };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("Books", result.Name);
    }

    [Fact]
    public async Task GetAllAsync_Returns_All_Categories()
    {
        _context.Categories.AddRange(
            new Category { Id = Guid.NewGuid(), Name = "A", CreatedAt = DateTime.UtcNow },
            new Category { Id = Guid.NewGuid(), Name = "B", CreatedAt = DateTime.UtcNow });
        await _context.SaveChangesAsync();

        var result = await _sut.GetAllAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task UpdateAsync_Modifies_Category()
    {
        var id = Guid.NewGuid();
        var category = new Category { Id = id, Name = "Old", CreatedAt = DateTime.UtcNow };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        category.Name = "New";
        category.UpdatedAt = DateTime.UtcNow;
        await _sut.UpdateAsync(category);

        var fromDb = await _context.Categories.FindAsync(id);
        Assert.NotNull(fromDb);
        Assert.Equal("New", fromDb.Name);
    }

    [Fact]
    public async Task DeleteAsync_Removes_Category()
    {
        var id = Guid.NewGuid();
        var category = new Category { Id = id, Name = "ToDelete", CreatedAt = DateTime.UtcNow };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        await _sut.DeleteAsync(category);

        var fromDb = await _context.Categories.FindAsync(id);
        Assert.Null(fromDb);
    }
}
