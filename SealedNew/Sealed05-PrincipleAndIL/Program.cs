using System.Reflection;

namespace Sealed05_PrincipleAndIL;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("=== 05 原理解析 ===");

        PrintTypeMetadata(typeof(FinalMessageWriter));
        PrintMethodMetadata(typeof(OptimizedMessageWriter).GetMethod(nameof(OptimizedMessageWriter.Write))!);

        Console.WriteLine();
        Console.WriteLine("推导:");
        Console.WriteLine("1. 类型 IsSealed=True，说明该类型不能再被继承");
        Console.WriteLine("2. 方法 IsFinal=True + IsVirtual=True，说明它是 sealed override");
        Console.WriteLine("3. JIT 看到这类信息后，更容易确认调用目标");
    }

    private static void PrintTypeMetadata(Type type)
    {
        Console.WriteLine($"Type: {type.Name}");
        Console.WriteLine($"IsClass = {type.IsClass}");
        Console.WriteLine($"IsSealed = {type.IsSealed}");
        Console.WriteLine($"IsAbstract = {type.IsAbstract}");
    }

    private static void PrintMethodMetadata(MethodInfo method)
    {
        Console.WriteLine();
        Console.WriteLine($"Method: {method.DeclaringType!.Name}.{method.Name}");
        Console.WriteLine($"IsVirtual = {method.IsVirtual}");
        Console.WriteLine($"IsFinal = {method.IsFinal}");
        Console.WriteLine($"GetBaseDefinition = {method.GetBaseDefinition().DeclaringType!.Name}.{method.GetBaseDefinition().Name}");
    }
}

public sealed class FinalMessageWriter
{
    public void Write(string message)
    {
        Console.WriteLine($"Final writer: {message}");
    }
}

public class MessageWriterBase
{
    public virtual void Write(string message)
    {
        Console.WriteLine($"Base writer: {message}");
    }
}

public class OptimizedMessageWriter : MessageWriterBase
{
    public sealed override void Write(string message)
    {
        Console.WriteLine($"Optimized writer: {message}");
    }
}
