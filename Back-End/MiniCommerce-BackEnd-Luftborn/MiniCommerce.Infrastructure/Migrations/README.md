# EF Core Migrations

Migrations live in **MiniCommerce.Infrastructure** (where `ApplicationDbContext` is defined).

## Apply migrations (when using SQL Server)

- **At startup:** With `UseInMemoryDatabase: false`, the API runs `Database.Migrate()` on startup and applies any pending migrations.
- **Manually:** From the solution folder:
  ```bash
  dotnet ef database update --project MiniCommerce.Infrastructure --startup-project MiniCommerce-BackEnd-Luftborn\MiniCommerce-BackEnd-Luftborn.csproj
  ```

## Add a new migration (requires dotnet-ef tool)

Install the tool once:
```bash
dotnet tool install --global dotnet-ef
```

From the solution folder (`MiniCommerce-BackEnd-Luftborn`):
```bash
dotnet ef migrations add YourMigrationName --project MiniCommerce.Infrastructure --startup-project MiniCommerce-BackEnd-Luftborn\MiniCommerce-BackEnd-Luftborn.csproj
```

Design-time: `DesignTimeDbContextFactory` in Infrastructure loads the connection string from the API project’s `appsettings.json` so migrations are generated for SQL Server even when the app uses InMemory for development.
