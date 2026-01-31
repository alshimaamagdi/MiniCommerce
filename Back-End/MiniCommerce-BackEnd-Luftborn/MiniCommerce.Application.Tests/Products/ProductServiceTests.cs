using MiniCommerce.Application.Common.Interfaces;
using MiniCommerce.Application.Products;
using MiniCommerce.Domain.Entities;
using Moq;
using Xunit;

namespace MiniCommerce.Application.Tests.Products;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly Mock<ICategoryRepository> _categoryRepoMock;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _productRepoMock = new Mock<IProductRepository>();
        _categoryRepoMock = new Mock<ICategoryRepository>();
        _sut = new ProductService(_productRepoMock.Object, _categoryRepoMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_Returns_All_Products_With_Category_Names()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics", CreatedAt = DateTime.UtcNow };
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "Laptop", Price = 999m, StockQuantity = 5, CategoryId = categoryId, CreatedAt = DateTime.UtcNow }
        };
        _productRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(products);
        _categoryRepoMock.Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>())).ReturnsAsync(category);

        var result = await _sut.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("Laptop", result[0].Name);
        Assert.Equal("Electronics", result[0].CategoryName);
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Null_When_Not_Found()
    {
        _productRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_Adds_Product_And_Returns_Dto()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Books", CreatedAt = DateTime.UtcNow };
        _categoryRepoMock.Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>())).ReturnsAsync(category);
        _productRepoMock.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken _) => p);

        var dto = new CreateProductDto("Book", "A book", 19.99m, 100, categoryId);
        var result = await _sut.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Book", result.Name);
        Assert.Equal(19.99m, result.Price);
        Assert.Equal(100, result.StockQuantity);
    }

    [Fact]
    public async Task UpdateAsync_Returns_Null_When_Not_Found()
    {
        _productRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var result = await _sut.UpdateAsync(Guid.NewGuid(), new UpdateProductDto("X", null, 0m, 0, Guid.NewGuid()));

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_Returns_False_When_Not_Found()
    {
        _productRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var result = await _sut.DeleteAsync(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task GetByCategoryIdAsync_Returns_Products_For_Category()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Toys", CreatedAt = DateTime.UtcNow };
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "Toy", Price = 10m, StockQuantity = 20, CategoryId = categoryId, CreatedAt = DateTime.UtcNow }
        };
        _productRepoMock.Setup(r => r.GetByCategoryIdAsync(categoryId, It.IsAny<CancellationToken>())).ReturnsAsync(products);
        _categoryRepoMock.Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>())).ReturnsAsync(category);

        var result = await _sut.GetByCategoryIdAsync(categoryId);

        Assert.Single(result);
        Assert.Equal("Toy", result[0].Name);
        Assert.Equal("Toys", result[0].CategoryName);
    }
}
