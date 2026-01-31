using MiniCommerce.Domain.Entities;
using Xunit;

namespace MiniCommerce.Domain.Tests.Entities;

public class CategoryTests
{
    [Fact]
    public void Category_Should_Inherit_From_BaseEntity()
    {
        var category = new Category();
        Assert.IsAssignableFrom<BaseEntity>(category);
    }

    [Fact]
    public void Category_Should_Have_Name_And_Description()
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Electronics",
            Description = "Electronic devices"
        };

        Assert.Equal("Electronics", category.Name);
        Assert.Equal("Electronic devices", category.Description);
    }

    [Fact]
    public void Category_Should_Have_Products_Collection()
    {
        var category = new Category();
        Assert.NotNull(category.Products);
        Assert.Empty(category.Products);
    }

    [Fact]
    public void Category_Should_Allow_Null_Description()
    {
        var category = new Category { Name = "Test", Description = null };
        Assert.Null(category.Description);
    }
}
