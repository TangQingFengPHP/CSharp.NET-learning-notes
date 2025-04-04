namespace VirtualAndAbstract;

public class DerivedClass : BaseClass
{
    public override void ShowMessage()
    {
        // 调用父类的 ShowMessage() 方法
        base.ShowMessage();
        Console.WriteLine("子类的 ShowMessage() 方法");
    }
}