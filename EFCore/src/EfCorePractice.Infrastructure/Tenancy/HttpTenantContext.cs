using EfCorePractice.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace EfCorePractice.Infrastructure.Tenancy;

public sealed class HttpTenantContext(IHttpContextAccessor httpContextAccessor) : ITenantContext
{
    public const string TenantHeaderName = "X-Tenant-Id";

    public int TenantId
    {
        get
        {
            if (httpContextAccessor.HttpContext?.Request.Headers.TryGetValue(TenantHeaderName, out var value) == true
                && int.TryParse(value, out var tenantId))
            {
                return tenantId;
            }

            return 1;
        }
    }

    public bool HasTenant =>
        httpContextAccessor.HttpContext?.Request.Headers.TryGetValue(TenantHeaderName, out var value) == true
        && !string.IsNullOrWhiteSpace(value);
}
