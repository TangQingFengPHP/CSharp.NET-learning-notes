namespace TaskFactory05_PitfallsAndBestPractices;

internal static class Program
{
    private static async Task Main()
    {
        Console.WriteLine("=== 05 常见坑与最佳实践 ===");
        Console.WriteLine();

        await ShowAsyncDelegatePitfallAsync();
        await ShowExceptionHandlingAsync();
        await ShowAttachedToParentAsync();
        await ShowFromAsyncAsync();

        Console.WriteLine();
        Console.WriteLine("要点:");
        Console.WriteLine("1. StartNew + async 委托会产生 Task<Task<T>>，需 Unwrap 或改用 Task.Run");
        Console.WriteLine("2. await 抛原始异常，Result/Wait 会包进 AggregateException 并阻塞线程");
        Console.WriteLine("3. AttachedToParent 让父任务等待子任务，Task.Run 默认 DenyChildAttach");
        Console.WriteLine("4. FromAsync 用于包装旧式 Begin/End API，新代码直接 await 异步方法");
    }

    private static async Task ShowAsyncDelegatePitfallAsync()
    {
        Console.WriteLine("--- async 委托嵌套 Task ---");

        Task<Task<string>> nestedTask = Task.Factory.StartNew(async () =>
        {
            await Task.Delay(300);
            return "异步结果";
        });

        Task<string> innerOnly = await nestedTask;
        Console.WriteLine($"  只 await 外层：Status={innerOnly.Status}（内部异步可能尚未完成）");

        Task<string> unwrapped = Task.Factory.StartNew(async () =>
        {
            await Task.Delay(300);
            return "Unwrap 结果";
        }).Unwrap();

        string result = await unwrapped;
        Console.WriteLine($"  Unwrap 后：{result}");

        string runResult = await Task.Run(async () =>
        {
            await Task.Delay(300);
            return "Task.Run 结果";
        });
        Console.WriteLine($"  Task.Run（推荐）：{runResult}");
        Console.WriteLine();
    }

    private static async Task ShowExceptionHandlingAsync()
    {
        Console.WriteLine("--- 异常处理 ---");

        Task errorTask = Task.Factory.StartNew(() => throw new InvalidOperationException("报表生成失败"));

        try
        {
            await errorTask;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"  await 捕获：{ex.Message}");
        }

        Task<int> errorResultTask = Task.Factory.StartNew<int>(() => throw new InvalidOperationException("计算失败"));

        try
        {
            _ = errorResultTask.Result;
        }
        catch (AggregateException ex)
        {
            Console.WriteLine($"  Result 捕获：{ex.InnerException?.Message}");
        }

        Console.WriteLine();
    }

    private static async Task ShowAttachedToParentAsync()
    {
        Console.WriteLine("--- AttachedToParent 父子任务 ---");

        Task parent = Task.Factory.StartNew(() =>
        {
            Console.WriteLine("  父任务开始");

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(400);
                Console.WriteLine("  子任务完成");
            }, TaskCreationOptions.AttachedToParent);

            Console.WriteLine("  父任务委托结束");
        });

        await parent;
        Console.WriteLine("  父任务真正完成");
        Console.WriteLine();
    }

    private static async Task ShowFromAsyncAsync()
    {
        Console.WriteLine("--- FromAsync 包装旧式 API ---");

        const string fileName = "orders-demo.txt";
        await File.WriteAllTextAsync(fileName, "hello task factory");

        await using FileStream stream = new(
            fileName,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true);

        byte[] buffer = new byte[1024];

        Task<int> readTask = Task.Factory.FromAsync(
            stream.BeginRead,
            stream.EndRead,
            buffer,
            0,
            buffer.Length,
            state: null);

        int bytesRead = await readTask;
        Console.WriteLine($"  读取字节数：{bytesRead}，内容：{System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead)}");

        File.Delete(fileName);
        Console.WriteLine();
    }
}
