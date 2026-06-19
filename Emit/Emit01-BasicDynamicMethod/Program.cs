using System.Reflection;
using System.Reflection.Emit;

namespace Emit01_BasicDynamicMethod;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("=== 01 DynamicMethod 与 IL 栈模型 ===");
        Console.WriteLine();

        ShowAddMethod();
        ShowIlStackExplanation();

        Console.WriteLine();
        Console.WriteLine("要点:");
        Console.WriteLine("1. DynamicMethod 适合在内存中生成单个轻量方法");
        Console.WriteLine("2. IL 是栈式模型：Ldarg 压栈 → Add 运算 → Ret 返回栈顶");
        Console.WriteLine("3. CreateDelegate 把动态方法转成可调用委托");
    }

    private static void ShowAddMethod()
    {
        Console.WriteLine("--- 动态生成 int Add(int a, int b) ---");

        Func<int, int, int> add = CreateAdd();
        Console.WriteLine($"  add(10, 20) = {add(10, 20)}");
        Console.WriteLine($"  add(3, 7) = {add(3, 7)}");
        Console.WriteLine();
    }

    private static void ShowIlStackExplanation()
    {
        Console.WriteLine("--- 等价 IL 指令 ---");
        Console.WriteLine("  Ldarg_0  // 加载参数 a");
        Console.WriteLine("  Ldarg_1  // 加载参数 b");
        Console.WriteLine("  Add      // 弹出两个 int，相加后压栈");
        Console.WriteLine("  Ret      // 返回栈顶 int");
        Console.WriteLine();
    }

    /// <summary>
    /// 运行时生成：int Add(int a, int b) => a + b;
    /// </summary>
    private static Func<int, int, int> CreateAdd()
    {
        DynamicMethod method = new(
            name: "Add",
            returnType: typeof(int),
            parameterTypes: [typeof(int), typeof(int)],
            m: typeof(Program).Module);

        ILGenerator il = method.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Add);
        il.Emit(OpCodes.Ret);

        return method.CreateDelegate<Func<int, int, int>>();
    }
}
