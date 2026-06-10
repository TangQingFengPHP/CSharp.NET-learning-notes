namespace TaskFactory01_BasicStartNew;

internal static class Program
{
    private static readonly StoreOrder[] Stores =
    [
        new("上海店", [120m, 88m, 300m]),
        new("杭州店", [66m, 180m, 210m]),
        new("深圳店", [520m, 45m, 99m]),
    ];

    private static async Task Main()
    {
        Console.WriteLine("=== 01 StartNew 基础与 Task.Run 对比 ===");
        Console.WriteLine($"主线程：{Environment.CurrentManagedThreadId}");
        Console.WriteLine();

        await ShowBasicStartNewAsync();
        await ShowStartNewWithResultAsync();
        await ShowTaskRunComparisonAsync();
        await ShowStateParameterAsync();

        Console.WriteLine();
        Console.WriteLine("要点:");
        Console.WriteLine("1. StartNew 参数更多，默认调度器不一定是线程池");
        Console.WriteLine("2. Task.Run 等价于 StartNew + DenyChildAttach + TaskScheduler.Default");
        Console.WriteLine("3. state 参数可减少闭包捕获，高频创建任务时略有优势");
    }

    private static async Task ShowBasicStartNewAsync()
    {
        Console.WriteLine("--- 基础 StartNew ---");

        Task task = Task.Factory.StartNew(() =>
        {
            Console.WriteLine($"  任务线程：{Environment.CurrentManagedThreadId}");
            Console.WriteLine("  开始生成订单报表");
        });

        await task;
        Console.WriteLine();
    }

    private static async Task ShowStartNewWithResultAsync()
    {
        Console.WriteLine("--- 带返回值的 StartNew ---");

        Task<StoreReport> reportTask = Task.Factory.StartNew(() =>
        {
            StoreOrder store = Stores[0];
            return new StoreReport(store.StoreName, store.Amounts.Sum(), Environment.CurrentManagedThreadId);
        });

        StoreReport report = await reportTask;
        Console.WriteLine($"  {report.StoreName} 销售额：{report.TotalAmount}，线程：{report.ThreadId}");
        Console.WriteLine();
    }

    private static async Task ShowTaskRunComparisonAsync()
    {
        Console.WriteLine("--- Task.Run 对比（更推荐用于普通后台任务）---");

        Task<StoreReport> runTask = Task.Run(() =>
        {
            StoreOrder store = Stores[0];
            return new StoreReport(store.StoreName, store.Amounts.Sum(), Environment.CurrentManagedThreadId);
        });

        StoreReport report = await runTask;
        Console.WriteLine($"  Task.Run 结果：{report.StoreName} 销售额={report.TotalAmount}，线程={report.ThreadId}");

        Task<StoreReport> explicitStartNew = Task.Factory.StartNew(
            () =>
            {
                StoreOrder store = Stores[0];
                return new StoreReport(store.StoreName, store.Amounts.Sum(), Environment.CurrentManagedThreadId);
            },
            CancellationToken.None,
            TaskCreationOptions.DenyChildAttach,
            TaskScheduler.Default);

        StoreReport explicitReport = await explicitStartNew;
        Console.WriteLine($"  显式 StartNew（等价 Task.Run 配置）：线程={explicitReport.ThreadId}");
        Console.WriteLine();
    }

    private static async Task ShowStateParameterAsync()
    {
        Console.WriteLine("--- state 参数减少闭包 ---");

        Task<StoreReport> stateTask = Task.Factory.StartNew(static state =>
        {
            var store = (StoreOrder)state!;
            return new StoreReport(store.StoreName, store.Amounts.Sum(), Environment.CurrentManagedThreadId);
        }, Stores[1]);

        StoreReport stateReport = await stateTask;
        Console.WriteLine($"  {stateReport.StoreName} 销售额：{stateReport.TotalAmount}");
        Console.WriteLine();
    }
}

internal sealed record StoreOrder(string StoreName, decimal[] Amounts);

internal sealed record StoreReport(string StoreName, decimal TotalAmount, int ThreadId);
