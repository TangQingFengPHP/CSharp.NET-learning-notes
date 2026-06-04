using EfCorePractice.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EfCorePractice.Api.Controllers;

[ApiController]
[Route("migrations")]
public class MigrationsController(MigrationService migrationService) : ControllerBase
{
    [HttpGet]
    public Task<Application.Models.MigrationInfoDto> Get(CancellationToken ct) =>
        migrationService.GetMigrationInfoAsync(ct);
}
