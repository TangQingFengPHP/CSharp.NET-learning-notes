using EfCorePractice.Domain.Common;

namespace EfCorePractice.Domain.Entities;

public abstract class BaseAuditableEntity : IAuditableEntity
{
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
