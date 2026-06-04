using EfCorePractice.Api.Middleware;
using EfCorePractice.Application;
using EfCorePractice.Infrastructure;
using EfCorePractice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "EF Core 实战 API",
        Version = "v1",
        Description = "覆盖 EF Core 9 常用知识点：迁移、全局过滤器、软删除、多租户、JSON 列、编译查询等"
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks();

var app = builder.Build();

var enableSwagger = builder.Configuration.GetValue("EnableSwagger", app.Environment.IsDevelopment());
if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "EF Core 实战 v1");
        options.RoutePrefix = "swagger";
    });
}

await app.Services.ApplyInfrastructureAsync(
    builder.Configuration,
    app.Logger);

app.UseMiddleware<GlobalExceptionMiddleware>();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();

public partial class Program;
