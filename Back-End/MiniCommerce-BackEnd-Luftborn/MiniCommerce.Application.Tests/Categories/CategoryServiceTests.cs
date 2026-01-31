using MiniCommerce.Application.Categories;
using MiniCommerce.Application.Common.Interfaces;
using MiniCommerce.Domain.Entities;
using Moq;
using Xunit;

namespace MiniCommerce.Application.Tests.Categories;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _repositoryMock;
    private readonly CategoryService _sut;

    public CategoryServiceTests()
    {
        _repositoryMock = new Mock<ICategoryRepository>();
        _sut = new CategoryService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_Returns_All_Categories()
    {
        var entities = new List<Category>
        {
            new() { Id = Guid.NewGuid(), Name = "A", Description = "Desc A", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "B", Description = "Desc B", CreatedAt = DateTime.UtcNow }
        };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(entities);

        var result = await _sut.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("A", result[0].Name);
        Assert.Equal("B", result[1].Name);
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Null_When_Not_Found()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Category?)null);

        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Dto_When_Found()
    {
        var id = Guid.NewGuid();
        var entity = new Category { Id = id, Name = "Electronics", Description = "Desc", CreatedAt = DateTime.UtcNow };
        _repositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var result = await _sut.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Electronics", result.Name);
    }

    [Fact]
    public async Task CreateAsync_Adds_Entity_And_Returns_Dto()
    {
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category c, CancellationToken _) => c);

        var dto = new CreateCategoryDto("New Category", "Description");

        var result = await _sut.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("New Category", result.Name);
        Assert.Equal("Description", result.Description);
        _repositoryMock.Verify(r => r.AddAsync(It.Is<Category>(c => c.Name == "New Category"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Returns_Null_When_Not_Found()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Category?)null);

        var result = await _sut.UpdateAsync(Guid.NewGuid(), new UpdateCategoryDto("X", "Y"));

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_Updates_And_Returns_Dto_When_Found()
    {
        var id = Guid.NewGuid();
        var entity = new Category { Id = id, Name = "Old", Description = "OldDesc", CreatedAt = DateTime.UtcNow };
        _repositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _sut.UpdateAsync(id, new UpdateCategoryDto("New", "NewDesc"));

        Assert.NotNull(result);
        Assert.Equal("New", result.Name);
        Assert.Equal("NewDesc", result.Description);
    }

    [Fact]
    public async Task DeleteAsync_Returns_False_When_Not_Found()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Category?)null);

        var result = await _sut.DeleteAsync(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_Returns_True_And_Deletes_When_Found()
    {
        var id = Guid.NewGuid();
        var entity = new Category { Id = id, Name = "ToDelete", CreatedAt = DateTime.UtcNow };
        _repositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _sut.DeleteAsync(id);

        Assert.True(result);
        _repositoryMock.Verify(r => r.DeleteAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }
}
