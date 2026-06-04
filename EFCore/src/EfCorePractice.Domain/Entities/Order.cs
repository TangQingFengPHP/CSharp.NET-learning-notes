using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using EfCorePractice.Domain.Enums;

namespace EfCorePractice.Domain.Entities;

[Table("orders")]
public class Order
{
    public long Id { get; set; }

    public long UserId { get; set; }

    [JsonIgnore]
    public User User { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string OrderNo { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public DateTime CreatedAt { get; set; }
}
