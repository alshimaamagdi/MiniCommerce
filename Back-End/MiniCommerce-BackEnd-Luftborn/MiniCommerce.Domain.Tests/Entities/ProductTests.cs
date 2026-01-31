using MiniCommerce.Domain.Entities;
using Xunit;

namespace MiniCommerce.Domain.Tests.Entities;

public class ProductTests
{
    [Fact]
    public void Product_Should_Inherit_From_BaseEntity()
    {
        var product = new Product();
        Assert.IsAssignableFrom<BaseEntity>(product);
    }

    [Fact]
    public void Product_Should_Have_Required_Properties()
    {
        var categoryId = Guid.NewGuid();
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Laptop",
            Description = "Gaming laptop",
            Price = 999.99m,
            StockQuantity = 10,
            CategoryId = categoryId
        };

        Assert.Equal("Laptop", product.Name);
        Assert.Equal("Gaming laptop", product.Description);
        Assert.Equal(999.99m, product.Price);
        Assert.Equal(10, product.StockQuantity);
        Assert.Equal(categoryId, product.CategoryId);
    }

    [Fact]
    public void Product_Should_Allow_Zero_Stock()
    {
        var product = new Product { StockQuantity = 0 };
        Assert.Equal(0, product.StockQuantity);
    }

    [Fact]
    public void Product_Should_Allow_Null_Description()
    {
        var product = new Product { Name = "Widget", Description = null };
        Assert.Null(product.Description);
    }
}
