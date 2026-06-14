namespace Thread01_BasicCreation;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("=== 01 创建线程、传参与获取结果 ===");
        Console.WriteLine();

        ShowBasicCreation();
        ShowLambdaParameter();
        ShowParameterizedStart();
        ShowReturnResult();

        Console.WriteLine();
        Console.WriteLine("要点:");
        Console.WriteLine("1. 创建 Thread 不等于启动，必须调用 Start()");
        Console.WriteLine("2. 同一 Thread 实例只能 Start 一次");
        Console.WriteLine("3. Thread 无返回值，需共享变量 + Join 后再读取");
    }

    private static void ShowBasicCreation()
    {
        Console.WriteLine("--- 创建并启动 ---");
        Console.WriteLine($"  主线程 ID：{Environment.CurrentManagedThreadId}");

        var worker = new Thread(() =>
        {
            Console.WriteLine($"  工作线程 ID：{Environment.CurrentManagedThreadId}");
            Console.WriteLine("  开始处理订单");
        })
        {
            Name = "OrderWorker"
        };

        worker.Start();
        Console.WriteLine("  主线程继续执行");
        worker.Join();
        Console.WriteLine("  工作线程已经结束");
        Console.WriteLine();
    }

    private static void ShowLambdaParameter()
    {
        Console.WriteLine("--- Lambda 传递参数 ---");

        string orderNo = "SO202606140001";
        decimal amount = 199.80m;

        var worker = new Thread(() =>
        {
            Console.WriteLine($"  处理订单：{orderNo}，金额：{amount}");
        });

        worker.Start();
        worker.Join();
        Console.WriteLine();
    }

    private static void ShowParameterizedStart()
    {
        Console.WriteLine("--- ParameterizedThreadStart ---");

        var worker = new Thread(static state =>
        {
            var args = (OrderArgs)state!;
            Console.WriteLine($"  处理订单：{args.OrderNo}，金额：{args.Amount}");
        });

        worker.Start(new OrderArgs("SO202606140002", 299.00m));
        worker.Join();
        Console.WriteLine();
    }

    private static void ShowReturnResult()
    {
        Console.WriteLine("--- 通过共享变量返回结果 ---");

        int result = 0;

        var worker = new Thread(() =>
        {
            result = Enumerable.Range(1, 100).Sum();
        });

        worker.Start();
        worker.Join();

        Console.WriteLine($"  1..100 求和 = {result}");
        Console.WriteLine();
    }
}

internal sealed record OrderArgs(string OrderNo, decimal Amount);
