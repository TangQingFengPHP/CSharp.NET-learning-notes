using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Emit05_InterfaceAndCaching;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("=== 05 动态接口实现与委托缓存 ===");
        Console.WriteLine();

        ShowDynamicCalculator();  // Emit04 生成普通类；本章在此基础上实现接口（Mock/代理的基础）
        ShowGetterCache();        // Emit 成本高，同一 getter 只生成一次
        ShowReflectionVsEmit();   // 缓存后的 Emit 委托 vs 反射 GetValue

        Console.WriteLine();
        Console.WriteLine("要点:");
        Console.WriteLine("1. DefineMethodOverride 把实现方法绑定到接口");
        Console.WriteLine("2. 实例方法 Ldarg_1/Ldarg_2 才是业务参数（Ldarg_0 是 this）");
        Console.WriteLine("3. Emit 生成成本高，必须用 ConcurrentDictionary 缓存委托");
    }

    private static void ShowDynamicCalculator()
    {
        Console.WriteLine("--- 动态实现 ICalculator ---");

        // 运行时生成 class RuntimeCalculator : ICalculator
        Type calculatorType = RuntimeCalculatorFactory.CreateCalculatorType();

        // 知道接口时可以直接强转，不必像 Emit04 那样全靠反射调方法
        var calculator = (ICalculator)(Activator.CreateInstance(calculatorType)
            ?? throw new InvalidOperationException("实例创建失败"));

        Console.WriteLine($"  calculator.Add(3, 5) = {calculator.Add(3, 5)}");
        Console.WriteLine();
    }

    private static void ShowGetterCache()
    {
        Console.WriteLine("--- Getter 委托缓存 ---");

        var user = new User { Id = 1, Name = "张三", Age = 20 };

        // 两次 GetOrCreate 相同 type + propertyName，返回同一个委托实例（不会重复 Emit）
        Func<object, object?> getName1 = EmitGetterCache.GetOrCreate(typeof(User), nameof(User.Name));
        Func<object, object?> getName2 = EmitGetterCache.GetOrCreate(typeof(User), nameof(User.Name));

        Console.WriteLine($"  两次获取同一 getter 是同一委托：{ReferenceEquals(getName1, getName2)}");
        Console.WriteLine($"  Name={getName1(user)}");
        Console.WriteLine();
    }

    private static void ShowReflectionVsEmit()
    {
        Console.WriteLine("--- 反射 vs Emit 性能粗测（100 万次）---");

        var user = new User { Id = 1, Name = "张三", Age = 20 };
        PropertyInfo nameProperty = typeof(User).GetProperty(nameof(User.Name))!;
        Func<object, object?> emitGetter = EmitGetterCache.GetOrCreate(typeof(User), nameof(User.Name));

        const int iterations = 1_000_000;

        // 反射：每次 GetValue 都要走元数据查找 + 校验，开销大
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            _ = nameProperty.GetValue(user);
        }
        sw.Stop();
        long reflectionMs = sw.ElapsedMilliseconds;

        // Emit 缓存委托：第一次生成 IL，之后就是普通方法调用
        sw.Restart();
        for (int i = 0; i < iterations; i++)
        {
            _ = emitGetter(user);
        }
        sw.Stop();
        long emitMs = sw.ElapsedMilliseconds;

        Console.WriteLine($"  反射 PropertyInfo.GetValue: {reflectionMs} ms");
        Console.WriteLine($"  Emit 缓存委托:             {emitMs} ms");
        Console.WriteLine();
    }
}

// 目标接口：运行时动态生成一个类来实现它（Mock 框架、Castle DynamicProxy 的简化版思路）
public interface ICalculator
{
    int Add(int a, int b);
}

public sealed class User
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public int Age { get; set; }
}

/// <summary>
/// 运行时生成等价于：
/// <code>
/// public class RuntimeCalculator : ICalculator
/// {
///     public int Add(int a, int b)
///     {
///         Console.WriteLine("  Add invoked");
///         return a + b;
///     }
/// }
/// </code>
/// </summary>
internal static class RuntimeCalculatorFactory
{
    public static Type CreateCalculatorType()
    {
        AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName("RuntimeCalculatorAssembly"),
            AssemblyBuilderAccess.RunAndCollect);

        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

        // DefineType 第 3 参数 = 基类 object，第 4 参数 = 实现的接口列表
        // 等价于：public class RuntimeCalculator : object, ICalculator
        TypeBuilder typeBuilder = moduleBuilder.DefineType(
            "RuntimeCalculator",
            TypeAttributes.Public | TypeAttributes.Class,
            typeof(object),
            [typeof(ICalculator)]);

        // 无参 public 构造函数，否则 Activator.CreateInstance 无法 new
        typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

        // 定义 Add 方法（类上的实现方法，尚未绑定到接口）
        MethodBuilder addMethod = typeBuilder.DefineMethod(
            "Add",
            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, // Virtual：接口实现必须是虚方法
            typeof(int),
            [typeof(int), typeof(int)]); // 参数 (int a, int b)

        ILGenerator il = addMethod.GetILGenerator();

        // --- 方法体 IL：Console.WriteLine("  Add invoked"); return a + b; ---

        il.Emit(OpCodes.Ldstr, "  Add invoked"); // 压入字符串常量
        il.Emit(OpCodes.Call, typeof(Console).GetMethod(
            nameof(Console.WriteLine),
            [typeof(string)])!); // 调静态方法 Console.WriteLine

        // 实例方法参数编号：Ldarg_0=this，Ldarg_1=a，Ldarg_2=b（不是 Ldarg_0/1！）
        il.Emit(OpCodes.Ldarg_1); // 加载参数 a
        il.Emit(OpCodes.Ldarg_2); // 加载参数 b
        il.Emit(OpCodes.Add);      // 相加
        il.Emit(OpCodes.Ret);      // 返回 int

        // 关键：把类上的 addMethod 声明为 ICalculator.Add 的实现
        // 没有这一步，类有 Add 方法但不 implements ICalculator
        MethodInfo interfaceMethod = typeof(ICalculator).GetMethod(nameof(ICalculator.Add))
            ?? throw new InvalidOperationException("接口方法不存在");

        typeBuilder.DefineMethodOverride(addMethod, interfaceMethod);

        return typeBuilder.CreateType()
            ?? throw new InvalidOperationException("动态类型创建失败");
    }
}

/// <summary>
/// Getter 委托缓存：Emit 生成一次，后续 GetOrCreate 直接返回已有委托。
/// 对应参考文档里的 GetOrCreateGetter + ConcurrentDictionary 模式。
/// </summary>
internal static class EmitGetterCache
{
    // key = "Emit05_InterfaceAndCaching.User.Name"，value = 动态生成的 getter 委托
    private static readonly ConcurrentDictionary<string, Func<object, object?>> Cache = new();

    public static Func<object, object?> GetOrCreate(Type type, string propertyName)
    {
        string key = type.FullName + "." + propertyName;

        // GetOrAdd：key 不存在时调用 CreateGetter 生成并缓存；已存在则直接返回，线程安全
        return Cache.GetOrAdd(key, _ => CreateGetter(type, propertyName));
    }

    /// <summary>
    /// 与 Emit02 CreateGetter 相同逻辑，此处单独复制是为演示「生成 + 缓存」分离。
    /// 等价于：object? Get(object instance) => ((T)instance).PropertyName;
    /// </summary>
    private static Func<object, object?> CreateGetter(Type type, string propertyName)
    {
        PropertyInfo property = type.GetProperty(propertyName)
            ?? throw new InvalidOperationException($"属性不存在：{propertyName}");

        MethodInfo getMethod = property.GetGetMethod()
            ?? throw new InvalidOperationException($"属性没有 getter：{propertyName}");

        DynamicMethod method = new(
            name: $"Get_{type.Name}_{propertyName}",
            returnType: typeof(object),
            parameterTypes: [typeof(object)],
            m: type.Module,
            skipVisibility: true);

        ILGenerator il = method.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);              // 加载 instance（object）
        il.Emit(OpCodes.Castclass, type);     // object → User
        il.Emit(OpCodes.Callvirt, getMethod);  // 调用 get_Name() / get_Age()

        if (property.PropertyType.IsValueType)
        {
            il.Emit(OpCodes.Box, property.PropertyType); // 值类型返回 object 需装箱
        }

        il.Emit(OpCodes.Ret);

        return method.CreateDelegate<Func<object, object?>>();
    }
}
