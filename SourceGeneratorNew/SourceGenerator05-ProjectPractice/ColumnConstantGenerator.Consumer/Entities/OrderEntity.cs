using ColumnMetadata;

namespace ColumnConstantGenerator.Consumer.Entities;

[GenerateColumnConstants]
public partial class OrderEntity
{
    public long OrderId { get; init; }

    public decimal Amount { get; init; }

    public string Status { get; init; } = string.Empty;
}
