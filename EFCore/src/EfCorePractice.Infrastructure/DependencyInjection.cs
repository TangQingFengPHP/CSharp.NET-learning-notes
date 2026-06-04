using EfCorePractice.Application.Abstractions;
using EfCorePractice.Application.Abstractions.Repositories;
using EfCorePractice.Application.Options;
using EfCorePractice.Infrastructure.Interceptors;
using EfCorePractice.Infrastructure.Persistence;
using EfCorePractice.Infrastructure.Repositories;
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
        services.Configure<SlowQueryOptions>(configuration.GetSection(SlowQueryOptions.SectionName));

        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, HttpTenantContext>();

        services.AddSingleton<AuditableEntityInterceptor>();
        services.AddSingleton<SlowQueryInterceptor>();
        services.AddSingleton<ISlowQueryMetrics, SlowQueryMetrics>();

        services.AddScoped<ICompiledQueryService, ScopedCompiledQueryService>();
        services.AddScoped<DatabaseSeeder>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        var useInMemory = configuration.GetValue<bool>("UseInMemoryDatabase");
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("缺少 ConnectionStrings:Default");
        var inMemoryName = configuration["InMemoryDatabaseName"] ?? "efcore_test";

        services.AddDbContext<AppDbContext>((sp, options) =>
            ConfigureDbContextOptions(
                options,
                sp,
                configuration,
                useInMemory,
                connectionString,
                inMemoryName,
                isReadOnly: false));

        services.AddDbContext<AppReadDbContext>((sp, options) =>
            ConfigureDbContextOptions(
                options,
                sp,
                configuration,
                useInMemory,
                connectionString,
                inMemoryName,
                isReadOnly: true));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IAppWriteDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IAppReadDbContext>(sp => sp.GetRequiredService<AppReadDbContext>());

        return services;
    }

    private static void ConfigureDbContextOptions(
        DbContextOptionsBuilder options,
        IServiceProvider sp,
        IConfiguration configuration,
        bool useInMemory,
        string connectionString,
        string inMemoryName,
        bool isReadOnly)
    {
        options.AddInterceptors(
            sp.GetRequiredService<SlowQueryInterceptor>(),
            sp.GetRequiredService<AuditableEntityInterceptor>());

        if (isReadOnly)
        {
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        if (useInMemory)
        {
            options.UseInMemoryDatabase(inMemoryName);
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
