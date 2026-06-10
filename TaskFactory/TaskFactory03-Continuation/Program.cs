namespace TaskFactory03_Continuation;

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
        Console.WriteLine("=== 03 ContinueWhenAll / ContinueWhenAny ===");
        Console.WriteLine();

        await ShowContinueWhenAllAsync();
        await ShowContinueWhenAnyAsync();
        await ShowModernAlternativeAsync();

        Console.WriteLine();
        Console.WriteLine("要点:");
        Console.WriteLine("1. ContinueWhenAll 等全部任务完成后执行延续");
        Console.WriteLine("2. ContinueWhenAny 取最快完成的任务结果");
        Console.WriteLine("3. 现代代码也可用 Task.WhenAll / Task.WhenAny，语义更贴近 async/await");
    }

    private static async Task ShowContinueWhenAllAsync()
    {
        Console.WriteLine("--- ContinueWhenAll：汇总所有门店 ---");

        Task<StoreReport>[] reportTasks = Stores
            .Select(store => Task.Factory.StartNew(static state =>
            {
                var item = (StoreOrder)state!;
                Thread.Sleep(300);
                return new StoreReport(item.StoreName, item.Amounts.Sum(), Environment.CurrentManagedThreadId);
            }, store))
            .ToArray();

        Task<decimal> totalTask = Task.Factory.ContinueWhenAll(reportTasks, completedTasks =>
        {
            decimal total = completedTasks.Sum(t => t.Result.TotalAmount);
            Console.WriteLine("  所有门店计算完成");
            return total;
        });

        decimal totalAmount = await totalTask;
        Console.WriteLine($"  总销售额：{totalAmount}");
        Console.WriteLine();
    }

    private static async Task ShowContinueWhenAnyAsync()
    {
        Console.WriteLine("--- ContinueWhenAny：取最快返回 ---");

        Task<StoreReport>[] raceTasks = Stores
            .Select((store, index) => Task.Factory.StartNew(static state =>
            {
                var data = ((StoreOrder Store, int Index))state!;
                Thread.Sleep((data.Index + 1) * 300);
                return new StoreReport(data.Store.StoreName, data.Store.Amounts.Sum(), Environment.CurrentManagedThreadId);
            }, (store, index)))
            .ToArray();

        Task<StoreReport> fastestTask = Task.Factory.ContinueWhenAny(raceTasks, completedTask => completedTask.Result);
        StoreReport fastest = await fastestTask;

        Console.WriteLine($"  最快返回：{fastest.StoreName}，销售额：{fastest.TotalAmount}");
        Console.WriteLine();
    }

    private static async Task ShowModernAlternativeAsync()
    {
        Console.WriteLine("--- 现代写法：Task.WhenAll ---");

        Task<StoreReport>[] tasks = Stores
            .Select(store => Task.Run(() => new StoreReport(store.StoreName, store.Amounts.Sum(), Environment.CurrentManagedThreadId)))
            .ToArray();

        StoreReport[] reports = await Task.WhenAll(tasks);
        decimal total = reports.Sum(x => x.TotalAmount);
        Console.WriteLine($"  WhenAll 汇总：{total}");
        Console.WriteLine();
    }
}

internal sealed record StoreOrder(string StoreName, decimal[] Amounts);

internal sealed record StoreReport(string StoreName, decimal TotalAmount, int ThreadId);
