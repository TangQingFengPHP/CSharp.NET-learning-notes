namespace EfCorePractice.Application.Abstractions;

public interface ITenantContext
{
    int TenantId { get; }

    bool HasTenant { get; }
}
