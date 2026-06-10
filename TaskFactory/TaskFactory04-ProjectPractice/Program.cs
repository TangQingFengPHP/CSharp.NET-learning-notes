namespace TaskFactory04_ProjectPractice;

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
        Console.WriteLine("=== 04 项目实践：门店订单报表 ===");
        Console.WriteLine($"主线程：{Environment.CurrentManagedThreadId}");
        Console.WriteLine();

        var generator = new StoreReportGenerator(Stores);

        await generator.GenerateAllReportsAsync();
        await generator.SummarizeWhenAllCompleteAsync();
        await generator.GetFastestReportAsync();
        await generator.GenerateWithCustomFactoryAsync();

        Console.WriteLine();
        Console.WriteLine("这个示例把 TaskFactory 用在：并行门店计算、ContinueWhenAll 汇总、ContinueWhenAny 竞速、自定义工厂统一策略。");
    }
}

/// <summary>
/// 模拟订单报表生成：不依赖数据库，专注 TaskFactory 调度练习。
/// </summary>
internal sealed class StoreReportGenerator(StoreOrder[] stores)
{
    public async Task GenerateAllReportsAsync()
    {
        Console.WriteLine("--- 并行计算各门店销售额 ---");

        Task<StoreReport>[] tasks = stores
            .Select(store => Task.Factory.StartNew(static state =>
            {
                var item = (StoreOrder)state!;
                Thread.Sleep(200);
                return new StoreReport(item.StoreName, item.Amounts.Sum(), Environment.CurrentManagedThreadId);
            }, store, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default))
            .ToArray();

        StoreReport[] reports = await Task.WhenAll(tasks);

        foreach (StoreReport report in reports)
        {
            Console.WriteLine($"  {report.StoreName}: {report.TotalAmount}（线程 {report.ThreadId}）");
        }

        Console.WriteLine();
    }

    public async Task SummarizeWhenAllCompleteAsync()
    {
        Console.WriteLine("--- ContinueWhenAll 汇总 ---");

        Task<StoreReport>[] reportTasks = stores
            .Select(store => Task.Factory.StartNew(static state =>
            {
                var item = (StoreOrder)state!;
                Thread.Sleep(200);
                return new StoreReport(item.StoreName, item.Amounts.Sum(), Environment.CurrentManagedThreadId);
            }, store))
            .ToArray();

        decimal total = await Task.Factory.ContinueWhenAll(reportTasks, completed =>
        {
            Console.WriteLine("  全部门店计算完成，开始汇总");
            return completed.Sum(t => t.Result.TotalAmount);
        });

        Console.WriteLine($"  总销售额：{total}");
        Console.WriteLine();
    }

    public async Task GetFastestReportAsync()
    {
        Console.WriteLine("--- ContinueWhenAny 最快门店 ---");

        Task<StoreReport>[] raceTasks = stores
            .Select((store, index) => Task.Factory.StartNew(static state =>
            {
                var data = ((StoreOrder Store, int Index))state!;
                Thread.Sleep((data.Index + 1) * 250);
                return new StoreReport(data.Store.StoreName, data.Store.Amounts.Sum(), Environment.CurrentManagedThreadId);
            }, (store, index)))
            .ToArray();

        StoreReport fastest = await Task.Factory.ContinueWhenAny(raceTasks, t => t.Result);
        Console.WriteLine($"  最快：{fastest.StoreName}，销售额 {fastest.TotalAmount}");
        Console.WriteLine();
    }

    public async Task GenerateWithCustomFactoryAsync()
    {
        Console.WriteLine("--- 自定义 TaskFactory 统一策略 ---");

        using var cts = new CancellationTokenSource();
        var factory = new TaskFactory(
            cts.Token,
            TaskCreationOptions.None,
            TaskContinuationOptions.None,
            TaskScheduler.Default);

        Task<StoreReport>[] tasks = stores
            .Select(store => factory.StartNew(static state =>
            {
                var item = (StoreOrder)state!;
                return new StoreReport(item.StoreName, item.Amounts.Sum(), Environment.CurrentManagedThreadId);
            }, store))
            .ToArray();

        StoreReport[] reports = await Task.WhenAll(tasks);

        foreach (StoreReport report in reports)
        {
            Console.WriteLine($"  [工厂] {report.StoreName}: {report.TotalAmount}");
        }

        Console.WriteLine();
    }
}

internal sealed record StoreOrder(string StoreName, decimal[] Amounts);

internal sealed record StoreReport(string StoreName, decimal TotalAmount, int ThreadId);
