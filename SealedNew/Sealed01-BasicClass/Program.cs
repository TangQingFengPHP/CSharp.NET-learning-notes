namespace Sealed01_BasicClass;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("=== 01 基础概念 ===");

        var provider = new TokenProvider("internal-service");
        Console.WriteLine(provider.IssueToken("alice"));

        var audit = new AuditRecord
        {
            Id = 1,
            Action = "Login",
            Operator = "alice"
        };

        Console.WriteLine($"AuditRecord => Id={audit.Id}, Action={audit.Action}, Operator={audit.Operator}");
        Console.WriteLine();
        Console.WriteLine("要点:");
        Console.WriteLine("1. sealed class 可以被实例化，但不能被继承");
        Console.WriteLine("2. struct 天生不可继承，因此无需 sealed struct");
        Console.WriteLine("3. abstract 与 sealed 不能同时用于同一个类");
    }
}

public sealed class TokenProvider
{
    private readonly string _issuer;

    public TokenProvider(string issuer)
    {
        _issuer = issuer;
    }

    public string IssueToken(string userName)
    {
        return $"issuer={_issuer};user={userName};issuedAt={DateTime.UtcNow:O}";
    }
}

public struct AuditRecord
{
    public int Id { get; set; }

    public string Action { get; set; }

    public string Operator { get; set; }
}

// 编译错误示例：
// public class CustomTokenProvider : TokenProvider { }
// CS0509: cannot derive from sealed type 'TokenProvider'

// 编译错误示例：
// public abstract sealed class InvalidType { }
// CS0418: abstract + sealed 只能出现在 static class 上下文对应的特殊语义里
