namespace Thread02_BackgroundAndJoin;

internal static class Program
{
    private static async Task Main()
    {
        Console.WriteLine("=== 02 前后台线程与 Join ===");
        Console.WriteLine();

        ShowJoinWithTimeout();
        ShowBackgroundThreadNote();
        await ShowSleepVsDelayAsync();

        Console.WriteLine();
        Console.WriteLine("要点:");
        Console.WriteLine("1. 默认 Thread 是前台线程，进程会等所有前台线程结束");
        Console.WriteLine("2. 后台线程可能随进程退出被直接终止，不能依赖收尾动作");
        Console.WriteLine("3. Join 超时只是不再等待，不会停止目标线程");
        Console.WriteLine("4. Thread.Sleep 阻塞线程，Task.Delay 不占用线程");
    }

    private static void ShowJoinWithTimeout()
    {
        Console.WriteLine("--- Join 与超时 ---");

        var worker = new Thread(() =>
        {
            Thread.Sleep(1500);
            Console.WriteLine("  任务完成");
        });

        worker.Start();
        Console.WriteLine("  开始等待");

        bool completed = worker.Join(TimeSpan.FromMilliseconds(500));

        if (!completed)
        {
            Console.WriteLine("  等待超时，线程仍在运行");
            worker.Join();
        }

        Console.WriteLine("  等待结束");
        Console.WriteLine();
    }

    private static void ShowBackgroundThreadNote()
    {
        Console.WriteLine("--- 后台线程说明 ---");
        Console.WriteLine("  IsBackground=true 时，所有前台线程结束后进程直接退出");
        Console.WriteLine("  后台线程的日志/刷新/提交可能不会执行");
        Console.WriteLine("  重要任务应明确 Stop + Join 等待收尾");
        Console.WriteLine();
    }

    private static async Task ShowSleepVsDelayAsync()
    {
        Console.WriteLine("--- Thread.Sleep vs Task.Delay ---");

        var sw = System.Diagnostics.Stopwatch.StartNew();

        await Task.Run(() =>
        {
            Console.WriteLine($"  Thread.Sleep 开始，线程 ID={Environment.CurrentManagedThreadId}");
            Thread.Sleep(500);
            Console.WriteLine("  Thread.Sleep 结束（阻塞了线程池线程）");
        });

        sw.Stop();
        Console.WriteLine($"  Sleep 耗时约 {sw.ElapsedMilliseconds} ms");

        sw.Restart();
        Console.WriteLine("  Task.Delay 开始（不占用等待线程）");
        await Task.Delay(500);
        sw.Stop();
        Console.WriteLine($"  Delay 耗时约 {sw.ElapsedMilliseconds} ms");
        Console.WriteLine();
    }
}
