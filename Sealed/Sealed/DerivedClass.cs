namespace Sealed;

public class DerivedClass : BaseClass 
{
    public sealed override void Show()
    {
        Console.WriteLine("DerivedClass Show");
    }
}

public class SubDerivedClass : DerivedClass
{
    // 不能重写 sealed 方法
    // public override void Show()
    // {
    //     
    // }
}