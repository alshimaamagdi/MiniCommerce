using Microsoft.AspNetCore.Mvc;
using MiniCommerce.Application.Products;
using MiniCommerce_BackEnd_Luftborn.Controllers;
using Moq;
using Xunit;

namespace MiniCommerce_BackEnd_Luftborn.Tests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _serviceMock;
    private readonly ProductsController _sut;

    public ProductsControllerTests()
    {
        _serviceMock = new Mock<IProductService>();
        _sut = new ProductsController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_Returns_Ok_With_List()
    {
        var categoryId = Guid.NewGuid();
        var list = new List<ProductDto>
        {
            new(Guid.NewGuid(), "Laptop", "Desc", 999m, 5, categoryId, "Electronics", DateTime.UtcNow)
        };
        _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(list);

        var result = await _sut.GetAll(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IReadOnlyList<ProductDto>>(okResult.Value);
        Assert.Single(returned);
        Assert.Equal("Laptop", returned[0].Name);
    }

    [Fact]
    public async Task GetById_Returns_NotFound_When_Null()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((ProductDto?)null);

        var result = await _sut.GetById(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetById_Returns_Ok_With_Dto_When_Found()
    {
        var id = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var dto = new ProductDto(id, "Laptop", "Gaming", 999m, 10, categoryId, "Electronics", DateTime.UtcNow);
        _serviceMock.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _sut.GetById(id, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<ProductDto>(okResult.Value);
        Assert.Equal(id, returned.Id);
        Assert.Equal(999m, returned.Price);
    }

    [Fact]
    public async Task GetByCategory_Returns_Ok_With_List()
    {
        var categoryId = Guid.NewGuid();
        var list = new List<ProductDto>
        {
            new(Guid.NewGuid(), "P1", null, 10m, 1, categoryId, "Toys", DateTime.UtcNow)
        };
        _serviceMock.Setup(s => s.GetByCategoryIdAsync(categoryId, It.IsAny<CancellationToken>())).ReturnsAsync(list);

        var result = await _sut.GetByCategory(categoryId, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IReadOnlyList<ProductDto>>(okResult.Value);
        Assert.Single(returned);
    }

    [Fact]
    public async Task Create_Returns_CreatedAtAction_With_Dto()
    {
        var id = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var created = new ProductDto(id, "Book", "A book", 19.99m, 100, categoryId, "Books", DateTime.UtcNow);
        _serviceMock.Setup(s => s.CreateAsync(It.IsAny<CreateProductDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(created);

        var result = await _sut.Create(new CreateProductDto("Book", "A book", 19.99m, 100, categoryId), CancellationToken.None);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(ProductsController.GetById), createdResult.ActionName);
        Assert.Equal(id, createdResult.RouteValues!["id"]);
    }

    [Fact]
    public async Task Update_Returns_NotFound_When_Null()
    {
        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateProductDto>(), It.IsAny<CancellationToken>())).ReturnsAsync((ProductDto?)null);

        var result = await _sut.Update(Guid.NewGuid(), new UpdateProductDto("X", null, 0m, 0, Guid.NewGuid()), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Update_Returns_Ok_With_Dto_When_Found()
    {
        var id = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var dto = new ProductDto(id, "Updated", null, 29.99m, 50, categoryId, "Books", DateTime.UtcNow);
        _serviceMock.Setup(s => s.UpdateAsync(id, It.IsAny<UpdateProductDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _sut.Update(id, new UpdateProductDto("Updated", null, 29.99m, 50, categoryId), CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(dto, okResult.Value);
    }

    [Fact]
    public async Task Delete_Returns_NotFound_When_False()
    {
        _serviceMock.Setup(s => s.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _sut.Delete(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_Returns_NoContent_When_True()
    {
        _serviceMock.Setup(s => s.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _sut.Delete(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }
}
