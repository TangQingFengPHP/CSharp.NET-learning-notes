namespace Sealed02_SealedOverride;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("=== 02 sealed override ===");

        PaymentProcessor processor = new WechatPaymentProcessor();
        processor.Process();

        Console.WriteLine();
        Console.WriteLine("解释:");
        Console.WriteLine("Base 定义虚方法 -> ChannelProcessor 重写并 sealed -> 再下层不能改核心流程");
    }
}

public abstract class PaymentProcessor
{
    public virtual void Process()
    {
        Console.WriteLine("Base: 校验请求");
        Console.WriteLine("Base: 记录审计日志");
    }
}

public class OnlinePaymentProcessor : PaymentProcessor
{
    public sealed override void Process()
    {
        Console.WriteLine("Online: 校验签名");
        Console.WriteLine("Online: 调用支付网关");
        Console.WriteLine("Online: 更新订单状态");
    }
}

public class WechatPaymentProcessor : OnlinePaymentProcessor
{
    public void PrintChannel()
    {
        Console.WriteLine("Wechat channel");
    }

    // 编译错误示例：
    // public override void Process() { }
    // CS0239: cannot override inherited member because it is sealed
}
