namespace Sealed04_ProjectPractice;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("=== 04 项目实践 ===");

        var order = new Order(1001, OrderStatus.PendingPayment);
        IOrderStateTransition transition = new OrderPaidStateTransition(new ConsoleOrderLogger());

        transition.Apply(order);

        Console.WriteLine($"订单 {order.Id} 当前状态: {order.Status}");
    }
}

public enum OrderStatus
{
    PendingPayment,
    Paid,
    Shipped,
    Completed
}

public sealed class Order
{
    public Order(long id, OrderStatus status)
    {
        Id = id;
        Status = status;
    }

    public long Id { get; }

    public OrderStatus Status { get; private set; }

    public void MarkAsPaid()
    {
        if (Status != OrderStatus.PendingPayment)
        {
            throw new InvalidOperationException($"当前状态 {Status} 不允许变更为 Paid");
        }

        Status = OrderStatus.Paid;
    }
}

public interface IOrderStateTransition
{
    void Apply(Order order);
}

public interface IOrderLogger
{
    void Log(string message);
}

public sealed class OrderPaidStateTransition : IOrderStateTransition
{
    private readonly IOrderLogger _logger;

    public OrderPaidStateTransition(IOrderLogger logger)
    {
        _logger = logger;
    }

    public void Apply(Order order)
    {
        order.MarkAsPaid();
        _logger.Log($"订单 {order.Id} 支付成功，状态已更新为 {order.Status}");
    }
}

public sealed class ConsoleOrderLogger : IOrderLogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[OrderLog] {message}");
    }
}
