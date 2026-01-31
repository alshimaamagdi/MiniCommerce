using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MiniCommerce.Infrastructure.Persistence;


public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var currentDir = Directory.GetCurrentDirectory();
        var candidates = new[]
        {
            Path.Combine(currentDir, "MiniCommerce-BackEnd-Luftborn"),
            Path.Combine(currentDir, "..", "MiniCommerce-BackEnd-Luftborn"),
            Path.Combine(currentDir, "..", "..", "MiniCommerce-BackEnd-Luftborn"),
            currentDir
        };
        var basePath = candidates.FirstOrDefault(d => File.Exists(Path.Combine(d, "appsettings.json"))) ?? currentDir;

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=.;Database=MiniCommerce;Trusted_Connection=True;MultipleActiveResultSets=true";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
