namespace EfCorePractice.Domain.Entities;

/// <summary>
/// 映射为 JSON 列（OwnsOne + ToJson），演示 EF Core 复杂类型序列化。
/// </summary>
public class UserContactProfile
{
    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = [];
}
