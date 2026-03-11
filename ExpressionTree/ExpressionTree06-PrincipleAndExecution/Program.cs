using System.Diagnostics;
using System.Linq.Expressions;

Console.WriteLine("=== 06 原理解析与执行机制 ===");

Func<int, int, int> directDelegate = (a, b) => a + b;
Expression<Func<int, int, int>> expression = (a, b) => a + b;

Console.WriteLine($"直接委托: {directDelegate(3, 5)}");
Console.WriteLine($"表达式树: {expression}");

var stopwatch = Stopwatch.StartNew();
var compiled = expression.Compile();
stopwatch.Stop();

Console.WriteLine($"编译表达式耗时(示意): {stopwatch.ElapsedTicks} ticks");
Console.WriteLine($"编译后执行结果: {compiled(3, 5)}");

Expression<Func<Order, bool>> orderExpr = order => order.Amount > 1000 && order.Status == "Paid";
Console.WriteLine();
Console.WriteLine($"适合 ORM 分析的表达式: {orderExpr}");
Console.WriteLine("原因: 这棵树可以被翻译成 SQL WHERE 条件，而不是只能在内存里运行。\n");

var compiledOrderExpr = orderExpr.Compile();
Console.WriteLine($"本地执行示例: {compiledOrderExpr(new Order { Amount = 1500, Status = "Paid" })}");

public sealed class Order
{
    public decimal Amount { get; init; }

    public string Status { get; init; } = string.Empty;
}
