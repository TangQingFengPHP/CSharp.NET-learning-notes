using System.Reflection;
using System.Reflection.Emit;

namespace Emit02_PropertyAccessor;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("=== 02 动态属性 Getter / Setter ===");
        Console.WriteLine();

        var user = new User { Id = 1, Name = "张三", Age = 20 };

        Func<object, object?> getName = EmitPropertyAccessor.CreateGetter(typeof(User), nameof(User.Name));
        Func<object, object?> getAge = EmitPropertyAccessor.CreateGetter(typeof(User), nameof(User.Age));

        Console.WriteLine($"  读取 Name={getName(user)}, Age={getAge(user)}");

        Action<object, object?> setName = EmitPropertyAccessor.CreateSetter(typeof(User), nameof(User.Name));
        Action<object, object?> setAge = EmitPropertyAccessor.CreateSetter(typeof(User), nameof(User.Age));

        setName(user, "李四");
        setAge(user, 28);

        Console.WriteLine($"  写入后 Name={user.Name}, Age={user.Age}");

        Console.WriteLine();
        Console.WriteLine("要点:");
        Console.WriteLine("1. Getter 返回值类型为 object 时，值类型属性需 Box");
        Console.WriteLine("2. Setter 接收 object 写入值类型时，需 Unbox_Any");
        Console.WriteLine("3. 实例方法 Ldarg_0 是 this，业务参数从 Ldarg_1 开始");
    }
}

public sealed class User
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public int Age { get; set; }
}

internal static class EmitPropertyAccessor
{
    public static Func<object, object?> CreateGetter(Type type, string propertyName)
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

        // 等价于：((User)instance).Name / .Age
        il.Emit(OpCodes.Ldarg_0);              // 加载第 1 个参数 instance（类型 object）
        il.Emit(OpCodes.Castclass, type);     // object → User，后续才能调用 User 上的 getter
        il.Emit(OpCodes.Callvirt, getMethod);  // 调用编译器生成的 get_Name / get_Age

        // 委托返回 object，值类型属性（如 int）需要装箱；引用类型（如 string）可直接返回
        if (property.PropertyType.IsValueType)
        {
            il.Emit(OpCodes.Box, property.PropertyType); // int → object（装箱）
        }

        il.Emit(OpCodes.Ret);

        return method.CreateDelegate<Func<object, object?>>();
    }

    public static Action<object, object?> CreateSetter(Type type, string propertyName)
    {
        PropertyInfo property = type.GetProperty(propertyName)
            ?? throw new InvalidOperationException($"属性不存在：{propertyName}");

        MethodInfo setMethod = property.GetSetMethod()
            ?? throw new InvalidOperationException($"属性没有 setter：{propertyName}");

        DynamicMethod method = new(
            name: $"Set_{type.Name}_{propertyName}",
            returnType: typeof(void),
            parameterTypes: [typeof(object), typeof(object)],
            m: type.Module,
            skipVisibility: true);

        ILGenerator il = method.GetILGenerator();

        // 等价于：((User)instance).Age = (int)value  或  ((User)instance).Name = (string)value
        il.Emit(OpCodes.Ldarg_0);              // 加载 instance（object）
        il.Emit(OpCodes.Castclass, type);     // object → User，不是装箱/拆箱，只是引用类型向下转换
        il.Emit(OpCodes.Ldarg_1);              // 加载 value（object），调用方传入的值

        // 第二个参数是 object，但 setter 需要具体类型，这里做「入参转换」（不是装箱）
        if (property.PropertyType.IsValueType)
        {
            // 值类型：如 Age=int，调用方传入的 28 已被装箱成 object，需拆箱还原为 int
            il.Emit(OpCodes.Unbox_Any, property.PropertyType);
        }
        else
        {
            // 引用类型：如 Name=string，把 object 转为 string（Castclass，不是 Box）
            il.Emit(OpCodes.Castclass, property.PropertyType);
        }

        // 调用编译器生成的 set_Age / set_Name，Emit 本身不实现赋值逻辑
        il.Emit(OpCodes.Callvirt, setMethod);
        il.Emit(OpCodes.Ret);

        return method.CreateDelegate<Action<object, object?>>();
    }
}
