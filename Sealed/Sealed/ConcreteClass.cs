namespace Sealed;

public class ConcreteClass : AbstractClass
{
    public sealed override void Display()
    {
        Console.WriteLine("ConcreteClass Display");
    }
}

public class SubConcreteClass : ConcreteClass
{
    // 不能重写 sealed 方法
    // public override void Display()
    // {
    // }
}