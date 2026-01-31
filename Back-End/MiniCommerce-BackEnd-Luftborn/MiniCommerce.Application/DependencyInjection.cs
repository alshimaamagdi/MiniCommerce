using Microsoft.Extensions.DependencyInjection;
using MiniCommerce.Application.Categories;
using MiniCommerce.Application.Products;

namespace MiniCommerce.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        return services;
    }
}
