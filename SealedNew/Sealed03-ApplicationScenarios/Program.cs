namespace Sealed03_ApplicationScenarios;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("=== 03 应用场景 ===");

        var money = new Currency("CNY");
        var signer = new JwtSigner("payments-api");
        var formatter = new OrderNoFormatter();

        Console.WriteLine($"值对象: {money.Code}");
        Console.WriteLine($"安全组件: {signer.Sign("order-1001")}");
        Console.WriteLine($"工具类: {formatter.Format(2026, 88)}");

        Console.WriteLine();
        Console.WriteLine("这些类型被 sealed 的原因不是为了炫技，而是为了限制错误继承。");
    }
}

public sealed class Currency
{
    public Currency(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Currency code is required.", nameof(code));
        }

        Code = code.ToUpperInvariant();
    }

    public string Code { get; }
}

public sealed class JwtSigner
{
    private readonly string _issuer;

    public JwtSigner(string issuer)
    {
        _issuer = issuer;
    }

    public string Sign(string payload)
    {
        return $"signed::{_issuer}::{payload}";
    }
}

public sealed class OrderNoFormatter
{
    public string Format(int year, int sequence)
    {
        return $"ORD-{year}-{sequence:D6}";
    }
}
