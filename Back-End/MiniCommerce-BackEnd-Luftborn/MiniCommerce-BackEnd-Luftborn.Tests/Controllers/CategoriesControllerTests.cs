using Microsoft.AspNetCore.Mvc;
using MiniCommerce.Application.Categories;
using MiniCommerce_BackEnd_Luftborn.Controllers;
using Moq;
using Xunit;

namespace MiniCommerce_BackEnd_Luftborn.Tests.Controllers;

public class CategoriesControllerTests
{
    private readonly Mock<ICategoryService> _serviceMock;
    private readonly CategoriesController _sut;

    public CategoriesControllerTests()
    {
        _serviceMock = new Mock<ICategoryService>();
        _sut = new CategoriesController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_Returns_Ok_With_List()
    {
        var list = new List<CategoryDto>
        {
            new(Guid.NewGuid(), "A", "Desc A", DateTime.UtcNow),
            new(Guid.NewGuid(), "B", "Desc B", DateTime.UtcNow)
        };
        _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(list);

        var result = await _sut.GetAll(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IReadOnlyList<CategoryDto>>(okResult.Value);
        Assert.Equal(2, returned.Count);
    }

    [Fact]
    public async Task GetById_Returns_NotFound_When_Null()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((CategoryDto?)null);

        var result = await _sut.GetById(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetById_Returns_Ok_With_Dto_When_Found()
    {
        var id = Guid.NewGuid();
        var dto = new CategoryDto(id, "Electronics", "Devices", DateTime.UtcNow);
        _serviceMock.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _sut.GetById(id, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<CategoryDto>(okResult.Value);
        Assert.Equal(id, returned.Id);
        Assert.Equal("Electronics", returned.Name);
    }

    [Fact]
    public async Task Create_Returns_CreatedAtAction_With_Dto()
    {
        var created = new CategoryDto(Guid.NewGuid(), "New", "Desc", DateTime.UtcNow);
        _serviceMock.Setup(s => s.CreateAsync(It.IsAny<CreateCategoryDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(created);

        var result = await _sut.Create(new CreateCategoryDto("New", "Desc"), CancellationToken.None);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(CategoriesController.GetById), createdResult.ActionName);
        Assert.Equal(created.Id, createdResult.RouteValues!["id"]);
        Assert.Same(created, createdResult.Value);
    }

    [Fact]
    public async Task Update_Returns_NotFound_When_Null()
    {
        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateCategoryDto>(), It.IsAny<CancellationToken>())).ReturnsAsync((CategoryDto?)null);

        var result = await _sut.Update(Guid.NewGuid(), new UpdateCategoryDto("X", "Y"), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Update_Returns_Ok_With_Dto_When_Found()
    {
        var id = Guid.NewGuid();
        var dto = new CategoryDto(id, "Updated", "Desc", DateTime.UtcNow);
        _serviceMock.Setup(s => s.UpdateAsync(id, It.IsAny<UpdateCategoryDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _sut.Update(id, new UpdateCategoryDto("Updated", "Desc"), CancellationToken.None);

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
