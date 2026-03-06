namespace Sealed06_SourceAnalysis;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("=== 06 源码分析 ===");

        IMessageSerializer serializer = new JsonMessageSerializer();
        Console.WriteLine(serializer.Serialize(new OrderCreated(1001, 99.8m)));

        Console.WriteLine();
        Console.WriteLine("源码分析结论:");
        Console.WriteLine("1. 抽象由接口 IMessageSerializer 承担");
        Console.WriteLine("2. 最终实现 JsonMessageSerializer 被 sealed，避免子类破坏序列化约定");
        Console.WriteLine("3. 想替换行为时，新增实现类，而不是继承现有最终实现类");
    }
}

public interface IMessageSerializer
{
    string Serialize<T>(T message);
}

public sealed class JsonMessageSerializer : IMessageSerializer
{
    public string Serialize<T>(T message)
    {
        var properties = typeof(T).GetProperties();
        var values = properties
            .Select(property => $"\"{property.Name}\":\"{property.GetValue(message)}\"");

        return "{" + string.Join(",", values) + "}";
    }
}

public sealed class OrderCreated
{
    public OrderCreated(long orderId, decimal amount)
    {
        OrderId = orderId;
        Amount = amount;
    }

    public long OrderId { get; }

    public decimal Amount { get; }
}

// 如果未来需要 XML 序列化，正确方式是新增实现：
// public sealed class XmlMessageSerializer : IMessageSerializer { ... }
// 而不是：public class CustomJsonSerializer : JsonMessageSerializer { ... }
