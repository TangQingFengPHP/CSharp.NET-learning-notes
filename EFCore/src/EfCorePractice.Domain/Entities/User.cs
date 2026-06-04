using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using EfCorePractice.Domain.Common;

namespace EfCorePractice.Domain.Entities;

public class User : BaseAuditableEntity, ISoftDelete, ITenantEntity
{
    public long Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    public int Age { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "ACTIVE";

    public int Version { get; set; }

    public int TenantId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public UserContactProfile Contact { get; set; } = new();

    [NotMapped]
    public string DisplayName => $"{Username} ({Email})";

    [JsonIgnore]
    public List<Order> Orders { get; set; } = [];
}
