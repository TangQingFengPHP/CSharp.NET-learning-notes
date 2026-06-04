using EfCorePractice.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EfCorePractice.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<UserService>();
        services.AddScoped<OrderService>();
        services.AddScoped<DemoService>();
        services.AddScoped<MigrationService>();
        return services;
    }
}
