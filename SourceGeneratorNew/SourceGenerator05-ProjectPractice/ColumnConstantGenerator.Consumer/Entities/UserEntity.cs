using ColumnMetadata;

namespace ColumnConstantGenerator.Consumer.Entities;

[GenerateColumnConstants]
public partial class UserEntity
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;
}
