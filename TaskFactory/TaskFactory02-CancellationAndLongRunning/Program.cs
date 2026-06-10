namespace TaskFactory02_CancellationAndLongRunning;

internal static class Program
{
    private static async Task Main()
    {
        Console.WriteLine("=== 02 取消令牌与 LongRunning ===");
        Console.WriteLine();

        await ShowCancellationAsync();
        await ShowLongRunningAsync();

        Console.WriteLine();
        Console.WriteLine("要点:");
        Console.WriteLine("1. CancellationToken 只是信号，任务内部要主动 ThrowIfCancellationRequested");
        Console.WriteLine("2. LongRunning 提示调度器创建独立线程，适合长时间阻塞任务");
        Console.WriteLine("3. 不要用 LongRunning 包装 async I/O，直接 await 原生异步 API 即可");
    }

    private static async Task ShowCancellationAsync()
    {
        Console.WriteLine("--- CancellationToken 取消 ---");

        using var cts = new CancellationTokenSource();

        Task cancelTask = Task.Factory.StartNew(() =>
        {
            for (int i = 1; i <= 10; i++)
            {
                cts.Token.ThrowIfCancellationRequested();
                Console.WriteLine($"  处理第 {i} 批订单");
                Thread.Sleep(300);
            }
        }, cts.Token);

        await Task.Delay(900);
        cts.Cancel();

        try
        {
            await cancelTask;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("  任务已取消");
        }

        Console.WriteLine();
    }

    private static async Task ShowLongRunningAsync()
    {
        Console.WriteLine("--- LongRunning 长任务 ---");

        Task longRunningTask = Task.Factory.StartNew(() =>
        {
            Console.WriteLine($"  LongRunning 线程：{Environment.CurrentManagedThreadId}");

            for (int i = 1; i <= 3; i++)
            {
                Console.WriteLine($"  后台队列第 {i} 次轮询");
                Thread.Sleep(400);
            }
        },
        CancellationToken.None,
        TaskCreationOptions.LongRunning,
        TaskScheduler.Default);

        await longRunningTask;
        Console.WriteLine();
    }
}
