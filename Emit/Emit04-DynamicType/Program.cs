using System.Reflection;
using System.Reflection.Emit;

namespace Emit04_DynamicType;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("=== 04 动态生成完整类型 ===");
        Console.WriteLine();

        // 1. 运行时「造」出一个 Type，等价于编译期存在的 class RuntimePerson
        Type personType = RuntimePersonTypeFactory.CreatePersonType();

        // 2. 动态类型没有编译期类型名，只能用 object + 反射创建实例
        //    等价于：new RuntimePerson("赵六")
        object person = Activator.CreateInstance(personType, "赵六")
            ?? throw new InvalidOperationException("实例创建失败");

        // 3. 通过反射读属性，等价于：person.Name
        Console.WriteLine($"  构造后 Name={personType.GetProperty("Name")!.GetValue(person)}");

        // 4. 通过反射写属性 + 调方法，等价于：person.Name = "钱七"; person.SayHello();
        personType.GetProperty("Name")!.SetValue(person, "钱七");
        Console.WriteLine($"  SayHello={personType.GetMethod("SayHello")!.Invoke(person, null)}");

        Console.WriteLine();
        Console.WriteLine("要点:");
        Console.WriteLine("1. 生成完整类需要 AssemblyBuilder → ModuleBuilder → TypeBuilder");
        Console.WriteLine("2. 字段、构造函数、属性 get/set、普通方法都要分别 Define + 发 IL");
        Console.WriteLine("3. CreateType() 后才能 Activator.CreateInstance 使用");
    }
}

internal static class RuntimePersonTypeFactory
{
    /// <summary>
    /// 运行时生成等价于：
    /// <code>
    /// public class RuntimePerson
    /// {
    ///     private string _name;
    ///
    ///     public RuntimePerson(string name) { _name = name; }
    ///
    ///     public string Name
    ///     {
    ///         get => _name;
    ///         set => _name = value;
    ///     }
    ///
    ///     public string SayHello() => "Hello " + _name;
    /// }
    /// </code>
    /// </summary>
    public static Type CreatePersonType()
    {
        // --- 第 1 层：动态程序集 ---
        // Emit03 用 DynamicMethod 只生成单个方法；Emit04 要生成完整 class，必须从程序集开始
        AssemblyName assemblyName = new("RuntimePersonAssembly");

        AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
            assemblyName,
            AssemblyBuilderAccess.RunAndCollect); // 内存运行，无前台引用时可被 GC 回收

        // --- 第 2 层：模块（一个程序集里通常至少一个模块）---
        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

        // --- 第 3 层：类型骨架 ---
        // TypeAttributes.Public | Class → public class RuntimePerson
        TypeBuilder typeBuilder = moduleBuilder.DefineType(
            "RuntimePerson",
            TypeAttributes.Public | TypeAttributes.Class);

        // --- 第 4 层：成员 ---
        // 字段、构造、属性、方法各自 Define，再分别发 IL
        FieldBuilder nameField = typeBuilder.DefineField(
            "_name",
            typeof(string),
            FieldAttributes.Private); // private string _name;

        DefineConstructor(typeBuilder, nameField);
        DefineNameProperty(typeBuilder, nameField);
        DefineSayHelloMethod(typeBuilder, nameField);

        // 所有成员定义完毕，「烘焙」成真正可用的 Type；在此之前不能 CreateInstance
        return typeBuilder.CreateType()
            ?? throw new InvalidOperationException("动态类型创建失败");
    }

    /// <summary>
    /// 生成构造函数：public RuntimePerson(string name) { _name = name; }
    /// 实例构造函数里：Ldarg_0 = this，Ldarg_1 = 第一个参数 name
    /// </summary>
    private static void DefineConstructor(TypeBuilder typeBuilder, FieldBuilder nameField)
    {
        ConstructorBuilder constructor = typeBuilder.DefineConstructor(
            MethodAttributes.Public,           // public
            CallingConventions.Standard,
            [typeof(string)]);                 // 参数 (string name)

        ILGenerator ctorIl = constructor.GetILGenerator();

        // 等价于：base();  // 必须先调 object 的无参构造
        ctorIl.Emit(OpCodes.Ldarg_0);                                              // 加载 this
        ctorIl.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)!); // Call 调基类构造（非虚方法）
        // 栈：[]

        // 等价于：this._name = name;
        ctorIl.Emit(OpCodes.Ldarg_0);       // 加载 this（Stfld 需要实例在栈上）
        ctorIl.Emit(OpCodes.Ldarg_1);       // 加载构造参数 name（实例构造里 Ldarg_1 是第一个业务参数）
        ctorIl.Emit(OpCodes.Stfld, nameField); // 写入 this._name = name
        // 栈：[]

        ctorIl.Emit(OpCodes.Ret);
    }

    /// <summary>
    /// 生成属性 Name 的 get/set。
    /// C# 属性编译后会变成 get_Name / set_Name 两个方法，Emit 也要手动 Define 并绑定。
    /// </summary>
    private static void DefineNameProperty(TypeBuilder typeBuilder, FieldBuilder nameField)
    {
        // 1. 声明属性本身（还没有方法体）
        PropertyBuilder nameProperty = typeBuilder.DefineProperty(
            "Name",
            PropertyAttributes.None,
            typeof(string),      // 属性类型
            Type.EmptyTypes);    // 索引器参数，普通属性为空

        // 2. 定义 getter：public string get_Name() { return this._name; }
        MethodBuilder getName = typeBuilder.DefineMethod(
            "get_Name",
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
            typeof(string),
            Type.EmptyTypes);

        ILGenerator getIl = getName.GetILGenerator();
        getIl.Emit(OpCodes.Ldarg_0);          // 加载 this
        getIl.Emit(OpCodes.Ldfld, nameField); // 读取 this._name，压栈
        getIl.Emit(OpCodes.Ret);              // 返回栈顶 string

        // 3. 定义 setter：public void set_Name(string value) { this._name = value; }
        MethodBuilder setName = typeBuilder.DefineMethod(
            "set_Name",
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
            typeof(void),
            [typeof(string)]);

        ILGenerator setIl = setName.GetILGenerator();
        setIl.Emit(OpCodes.Ldarg_0);         // 加载 this
        setIl.Emit(OpCodes.Ldarg_1);         // 加载 value（实例方法 Ldarg_1 = 第一个参数）
        setIl.Emit(OpCodes.Stfld, nameField); // this._name = value
        setIl.Emit(OpCodes.Ret);

        // 4. 把 get/set 方法绑定到 Name 属性，否则只是两个普通方法，不是属性
        nameProperty.SetGetMethod(getName);
        nameProperty.SetSetMethod(setName);
    }

    /// <summary>
    /// 生成实例方法：public string SayHello() { return "Hello " + _name; }
    /// </summary>
    private static void DefineSayHelloMethod(TypeBuilder typeBuilder, FieldBuilder nameField)
    {
        MethodBuilder sayHello = typeBuilder.DefineMethod(
            "SayHello",
            MethodAttributes.Public,
            typeof(string),
            Type.EmptyTypes);

        ILGenerator sayIl = sayHello.GetILGenerator();

        // string.Concat("Hello ", this._name) 的 IL 等价写法：
        sayIl.Emit(OpCodes.Ldstr, "Hello ");  // 压入字符串常量 → 栈：["Hello "]
        sayIl.Emit(OpCodes.Ldarg_0);          // 加载 this → 栈：["Hello ", this]
        sayIl.Emit(OpCodes.Ldfld, nameField); // 读 _name → 栈：["Hello ", name值]
        sayIl.Emit(OpCodes.Call, typeof(string).GetMethod(
            nameof(string.Concat),
            [typeof(string), typeof(string)])!); // 静态 Concat，栈：["Hello 赵六"]
        sayIl.Emit(OpCodes.Ret);              // 返回拼接结果
    }
}
