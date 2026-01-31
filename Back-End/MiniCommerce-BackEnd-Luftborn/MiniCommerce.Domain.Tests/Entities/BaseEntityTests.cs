using MiniCommerce.Domain.Entities;
using Xunit;

namespace MiniCommerce.Domain.Tests.Entities;

public class BaseEntityTests
{
    [Fact]
    public void BaseEntity_Should_Have_Id_CreatedAt_UpdatedAt()
    {
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow.AddDays(-1);
        var category = new Category
        {
            Id = id,
            CreatedAt = createdAt,
            UpdatedAt = null
        };

        Assert.Equal(id, category.Id);
        Assert.Equal(createdAt, category.CreatedAt);
        Assert.Null(category.UpdatedAt);
    }

    [Fact]
    public void BaseEntity_UpdatedAt_Can_Be_Set()
    {
        var updatedAt = DateTime.UtcNow;
        var category = new Category { UpdatedAt = updatedAt };
        Assert.Equal(updatedAt, category.UpdatedAt);
    }
}
