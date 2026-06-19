using System.Reflection;
using System.Reflection.Emit;

namespace Emit03_ObjectMapper;

internal static class Program
{
    // 映射委托只生成一次，后续反复调用 CachedMapper(user) 即可，避免每次映射都重新 Emit
    private static readonly Func<User, UserDto> CachedMapper = EmitObjectMapper.CreateMapper<User, UserDto>();

    private static void Main()
    {
        Console.WriteLine("=== 03 动态对象映射器 ===");
        Console.WriteLine();

        var user = new User { Id = 100, Name = "王五", Age = 30 };

        // CachedMapper 是运行时动态生成的 Func<User, UserDto>，等价于手写逐字段赋值
        UserDto dto = CachedMapper(user);
        Console.WriteLine($"  User  -> Id={user.Id}, Name={user.Name}, Age={user.Age}");
        Console.WriteLine($"  Dto   -> Id={dto.Id}, Name={dto.Name}"); // Age 不在 Dto 中，自动跳过

        Console.WriteLine();
        Console.WriteLine("要点:");
        Console.WriteLine("1. 映射器按同名同类型属性逐字段生成 get/set IL");
        Console.WriteLine("2. 生成一次后缓存委托，不要每次映射都重新 Emit");
        Console.WriteLine("3. 这是 AutoMapper 等框架底层的简化版思路");
    }
}

// 源类型：包含 Id、Name、Age 三个属性
public sealed class User
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public int Age { get; set; }
}

// 目标类型：只有 Id、Name，没有 Age → 映射时 Age 会被 foreach 逻辑自动跳过
public sealed class UserDto
{
    public int Id { get; set; }

    public string Name { get; set; } = "";
}

internal static class EmitObjectMapper
{
    /// <summary>
    /// 运行时生成映射方法，等价于手写：
    /// <code>
    /// UserDto Map(User source)
    /// {
    ///     var target = new UserDto();
    ///     target.Id = source.Id;
    ///     target.Name = source.Name;
    ///     return target;
    /// }
    /// </code>
    /// </summary>
    public static Func<TSource, TTarget> CreateMapper<TSource, TTarget>()
        where TTarget : new() // 约束目标类型有无参构造函数，因为 IL 里会 newobj
    {
        Type sourceType = typeof(TSource); // 例如 User
        Type targetType = typeof(TTarget); // 例如 UserDto

        // 1. 创建动态方法：签名 Func&lt;TSource, TTarget&gt;，即 TTarget Map(TSource source)
        DynamicMethod method = new(
            name: $"Map_{sourceType.Name}_To_{targetType.Name}", // 调试用名称，如 Map_User_To_UserDto
            returnType: targetType,                               // 返回 UserDto
            parameterTypes: [sourceType],                          // 唯一参数 User source → IL 里是 Ldarg_0
            m: targetType.Module,                                 // 宿主模块，决定动态方法归属哪个程序集
            skipVisibility: true);                                // 允许访问 internal/private 成员

        ILGenerator il = method.GetILGenerator();

        // 2. 声明局部变量 UserDto targetLocal，用来保存 new UserDto() 的结果
        //    等价于 C# 里的：UserDto target = new UserDto();
        LocalBuilder targetLocal = il.DeclareLocal(targetType);

        // 3. 获取目标类型的无参构造函数，后面用 Newobj 指令调用
        ConstructorInfo ctor = targetType.GetConstructor(Type.EmptyTypes)
            ?? throw new InvalidOperationException($"{targetType.Name} 缺少无参构造函数");

        // --- IL 段 1：创建目标对象 ---
        // 栈变化：[] → [UserDto实例]
        il.Emit(OpCodes.Newobj, ctor);        // 调用 new UserDto()，实例压栈
        il.Emit(OpCodes.Stloc, targetLocal);  // 弹出栈顶实例，存入局部变量 targetLocal
        // 此时栈：[]

        // 4. 遍历目标类型的每个属性，尝试从源类型找同名同类型属性并生成赋值 IL
        foreach (PropertyInfo targetProperty in targetType.GetProperties())
        {
            // 目标属性必须可写（有 setter），否则无法赋值
            if (!targetProperty.CanWrite)
            {
                continue;
            }

            // 在源类型中找同名属性，例如 target.Id → source.Id
            PropertyInfo? sourceProperty = sourceType.GetProperty(targetProperty.Name);
            if (sourceProperty is null || !sourceProperty.CanRead)
            {
                continue; // 源没有同名属性或不可读，跳过（如 UserDto 没有 Age 对应的赋值）
            }

            // 类型必须完全一致才映射，不做 int→long 等隐式转换
            if (sourceProperty.PropertyType != targetProperty.PropertyType)
            {
                continue;
            }

            // 拿到 getter/setter 的 MethodInfo，后面 Callvirt 调用
            MethodInfo getMethod = sourceProperty.GetGetMethod()
                ?? throw new InvalidOperationException($"源属性没有 getter：{sourceProperty.Name}");

            MethodInfo setMethod = targetProperty.GetSetMethod()
                ?? throw new InvalidOperationException($"目标属性没有 setter：{targetProperty.Name}");

            // --- IL 段 2：生成一条赋值语句 target.Xxx = source.Xxx ---
            // 以 Id 为例，等价于：target.Id = source.Id;
            //
            // 实例方法 Callvirt 要求栈顶顺序为：[实例, 参数...]
            // set_Id(int value) 需要：栈底=target实例，栈顶=要写入的int值

            il.Emit(OpCodes.Ldloc, targetLocal);  // 加载 target 实例 → 栈：[target]
            il.Emit(OpCodes.Ldarg_0);             // 加载 source 参数 → 栈：[target, source]
            il.Emit(OpCodes.Callvirt, getMethod); // 调用 source.get_Id() → 栈：[target, id值]
            il.Emit(OpCodes.Callvirt, setMethod); // 调用 target.set_Id(id值) → 栈：[]
        }

        // --- IL 段 3：返回目标对象 ---
        // 栈变化：[] → [UserDto实例] → 返回
        il.Emit(OpCodes.Ldloc, targetLocal); // 加载局部变量 target → 栈：[target]
        il.Emit(OpCodes.Ret);               // 返回栈顶 target

        // 5. 把动态 IL 编译成可调用委托，之后 CachedMapper(user) 就是直接调这段生成的代码
        return method.CreateDelegate<Func<TSource, TTarget>>();
    }
}
