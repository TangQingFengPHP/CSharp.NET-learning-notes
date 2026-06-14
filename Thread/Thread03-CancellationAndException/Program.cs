namespace Thread03_CancellationAndException;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("=== 03 协作取消与异常处理 ===");
        Console.WriteLine();

        ShowCancellation();
        ShowInterrupt();
        ShowExceptionCapture();

        Console.WriteLine();
        Console.WriteLine("要点:");
        Console.WriteLine("1. 用 CancellationToken 协作取消，不要强行终止线程");
        Console.WriteLine("2. Interrupt 只影响 Sleep/Wait/Join 等待状态");
        Console.WriteLine("3. 线程内异常不会自动传回主线程，需在入口捕获");
    }

    private static void ShowCancellation()
    {
        Console.WriteLine("--- CancellationToken 协作取消 ---");

        using var cts = new CancellationTokenSource();

        var worker = new Thread(() =>
        {
            CancellationToken token = cts.Token;

            while (!token.IsCancellationRequested)
            {
                Console.WriteLine($"  {DateTime.Now:HH:mm:ss.fff} 检查新订单");

                if (token.WaitHandle.WaitOne(TimeSpan.FromMilliseconds(400)))
                {
                    break;
                }
            }

            Console.WriteLine("  工作线程正常退出");
        })
        {
            Name = "OrderPoller"
        };

        worker.Start();
        Thread.Sleep(1200);
        cts.Cancel();
        worker.Join();
        Console.WriteLine();
    }

    private static void ShowInterrupt()
    {
        Console.WriteLine("--- Interrupt 中断等待 ---");

        var worker = new Thread(() =>
        {
            try
            {
                Console.WriteLine("  线程进入无限等待");
                Thread.Sleep(Timeout.Infinite);
            }
            catch (ThreadInterruptedException)
            {
                Console.WriteLine("  线程等待被 Interrupt 中断");
            }
        });

        worker.Start();
        Thread.Sleep(400);
        worker.Interrupt();
        worker.Join();
        Console.WriteLine();
    }

    private static void ShowExceptionCapture()
    {
        Console.WriteLine("--- 线程异常捕获 ---");

        Exception? workerException = null;

        var worker = new Thread(() =>
        {
            try
            {
                throw new InvalidOperationException("订单处理失败");
            }
            catch (Exception ex)
            {
                workerException = ex;
            }
        });

        worker.Start();
        worker.Join();

        if (workerException is not null)
        {
            Console.WriteLine($"  主线程收到异常：{workerException.Message}");
        }

        Console.WriteLine("  对比：Task 的异常可通过 await 自然传播");
        Console.WriteLine();
    }
}
