using EfCorePractice.Application.Abstractions;
using EfCorePractice.Infrastructure.Interceptors;
using EfCorePractice.Infrastructure.Persistence;
using EfCorePractice.Infrastructure.Seed;
using EfCorePractice.Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EfCorePractice.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, HttpTenantContext>();
        services.AddSingleton<AuditableEntityInterceptor>();
        services.AddScoped<ICompiledQueryService, ScopedCompiledQueryService>();
        services.AddScoped<DatabaseSeeder>();

        var useInMemory = configuration.GetValue<bool>("UseInMemoryDatabase");
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("缺少 ConnectionStrings:Default");

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());

            if (useInMemory)
            {
                options.UseInMemoryDatabase(configuration["InMemoryDatabaseName"] ?? "efcore_test");
            }
            else
            {
                options.UseMySql(
                    connectionString,
                    new MySqlServerVersion(new Version(8, 4, 0)),
                    mySql => mySql.EnableRetryOnFailure(maxRetryCount: 3));
            }

            if (configuration["ASPNETCORE_ENVIRONMENT"] == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.LogTo(Console.WriteLine, LogLevel.Information);
            }
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        return services;
    }

    public static async Task ApplyInfrastructureAsync(
        this IServiceProvider services,
        IConfiguration configuration,
        ILogger logger,
        CancellationToken ct = default)
    {
        await using var scope = services.CreateAsyncScope();
        var sp = scope.ServiceProvider;

        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            await sp.GetRequiredService<DatabaseSeeder>().SeedAsync(ct);
            return;
        }

        if (!configuration.GetValue("ApplyMigrationsOnStartup", true))
        {
            return;
        }

        var db = sp.GetRequiredService<AppDbContext>();
        var pending = await db.Database.GetPendingMigrationsAsync(ct);
        if (pending.Any())
        {
            logger.LogInformation("正在应用迁移: {Migrations}", string.Join(", ", pending));
            await db.Database.MigrateAsync(ct);
        }

        await sp.GetRequiredService<DatabaseSeeder>().SeedAsync(ct);
    }
}
