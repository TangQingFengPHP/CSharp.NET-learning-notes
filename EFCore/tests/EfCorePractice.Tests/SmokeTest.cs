using EfCorePractice.Application.Services;
using EfCorePractice.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EfCorePractice.Tests;

public class SmokeTest : IClassFixture<EfCorePracticeWebApplicationFactory>
{
    private readonly EfCorePracticeWebApplicationFactory _factory;

    public SmokeTest(EfCorePracticeWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Host_starts_and_can_create_user()
    {
        using var scope = _factory.Services.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<UserService>();
        var id = await userService.CreateAsync("测试用户", $"test-{Guid.NewGuid():N}@example.com", 22);
        Assert.True(id > 0);
    }

    [Fact]
    public async Task Migration_info_endpoint_works()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/migrations");
        response.EnsureSuccessStatusCode();
    }
}

public sealed class EfCorePracticeWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.UseSetting("UseInMemoryDatabase", "true");
        builder.UseSetting("InMemoryDatabaseName", $"efcore_smoke_{Guid.NewGuid()}");
        builder.UseSetting("ApplyMigrationsOnStartup", "false");
    }
}
