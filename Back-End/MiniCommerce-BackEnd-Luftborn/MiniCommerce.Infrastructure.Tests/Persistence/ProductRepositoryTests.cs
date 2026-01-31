using Microsoft.EntityFrameworkCore;
using MiniCommerce.Domain.Entities;
using MiniCommerce.Infrastructure.Persistence;
using Xunit;

namespace MiniCommerce.Infrastructure.Tests.Persistence;

public class ProductRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ProductRepository _sut;
    private readonly CategoryRepository _categoryRepository;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "ProductRepo_" + Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _categoryRepository = new CategoryRepository(_context);
        _sut = new ProductRepository(_context);
    }

    public void Dispose() => _context.Dispose();

    private async Task<Category> CreateCategoryAsync(string name = "Default")
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedAt = DateTime.UtcNow
        };
        await _categoryRepository.AddAsync(category);
        return category;
    }

    [Fact]
    public async Task AddAsync_Adds_Product_To_Db()
    {
        var category = await CreateCategoryAsync();
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Laptop",
            Price = 999m,
            StockQuantity = 10,
            CategoryId = category.Id,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _sut.AddAsync(product);

        Assert.Same(product, result);
        var fromDb = await _context.Products.FindAsync(product.Id);
        Assert.NotNull(fromDb);
        Assert.Equal("Laptop", fromDb.Name);
        Assert.Equal(category.Id, fromDb.CategoryId);
    }

    [Fact]
    public async Task GetByCategoryIdAsync_Returns_Only_Products_In_Category()
    {
        var cat1 = await CreateCategoryAsync("Cat1");
        var cat2 = await CreateCategoryAsync("Cat2");
        var p1 = new Product { Id = Guid.NewGuid(), Name = "P1", Price = 1m, StockQuantity = 1, CategoryId = cat1.Id, CreatedAt = DateTime.UtcNow };
        var p2 = new Product { Id = Guid.NewGuid(), Name = "P2", Price = 2m, StockQuantity = 2, CategoryId = cat1.Id, CreatedAt = DateTime.UtcNow };
        var p3 = new Product { Id = Guid.NewGuid(), Name = "P3", Price = 3m, StockQuantity = 3, CategoryId = cat2.Id, CreatedAt = DateTime.UtcNow };
        await _sut.AddAsync(p1);
        await _sut.AddAsync(p2);
        await _sut.AddAsync(p3);

        var result = await _sut.GetByCategoryIdAsync(cat1.Id);

        Assert.Equal(2, result.Count);
        Assert.True(result.All(p => p.CategoryId == cat1.Id));
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Null_When_Not_Found()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_Modifies_Product()
    {
        var category = await CreateCategoryAsync();
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Old",
            Price = 10m,
            StockQuantity = 5,
            CategoryId = category.Id,
            CreatedAt = DateTime.UtcNow
        };
        await _sut.AddAsync(product);

        product.Name = "Updated";
        product.Price = 20m;
        product.UpdatedAt = DateTime.UtcNow;
        await _sut.UpdateAsync(product);

        var fromDb = await _context.Products.FindAsync(product.Id);
        Assert.NotNull(fromDb);
        Assert.Equal("Updated", fromDb.Name);
        Assert.Equal(20m, fromDb.Price);
    }

    [Fact]
    public async Task DeleteAsync_Removes_Product()
    {
        var category = await CreateCategoryAsync();
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "ToDelete",
            Price = 1m,
            StockQuantity = 1,
            CategoryId = category.Id,
            CreatedAt = DateTime.UtcNow
        };
        await _sut.AddAsync(product);

        await _sut.DeleteAsync(product);

        var fromDb = await _context.Products.FindAsync(product.Id);
        Assert.Null(fromDb);
    }
}
