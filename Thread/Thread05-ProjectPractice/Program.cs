using System.Collections.Concurrent;

namespace Thread05_ProjectPractice;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("=== 05 项目实践：专用订单工作线程 ===");
        Console.WriteLine();

        using var worker = new OrderWorker();

        worker.Start();

        worker.Enqueue(new OrderJob("SO001", 99.00m));
        worker.Enqueue(new OrderJob("SO002", 268.00m));
        worker.Enqueue(new OrderJob("SO003", -1.00m));
        worker.Enqueue(new OrderJob("SO004", 520.00m));

        worker.Stop();

        Console.WriteLine();
        Console.WriteLine("这个示例展示了 Thread 的合理用法：长期专用线程 + 阻塞队列 + 明确停止 + Join 收尾。");
    }
}

public sealed record OrderJob(string OrderNo, decimal Amount);

/// <summary>
/// 专用订单工作线程：BlockingCollection 阻塞队列，同步处理，单个任务异常不打崩线程。
/// </summary>
public sealed class OrderWorker : IDisposable
{
    private readonly BlockingCollection<OrderJob> _queue = new();
    private readonly Thread _thread;
    private bool _started;
    private bool _stopped;
    private bool _disposed;

    public OrderWorker()
    {
        _thread = new Thread(Run)
        {
            Name = "OrderWorker",
            IsBackground = false
        };
    }

    public void Start()
    {
        ThrowIfDisposed();

        if (_started)
        {
            throw new InvalidOperationException("工作线程不能重复启动");
        }

        if (_stopped)
        {
            throw new InvalidOperationException("已经停止的工作线程不能重新启动");
        }

        _started = true;
        _thread.Start();
    }

    public void Enqueue(OrderJob job)
    {
        ThrowIfDisposed();

        if (_stopped || _queue.IsAddingCompleted)
        {
            throw new InvalidOperationException("工作线程已经停止接收任务");
        }

        _queue.Add(job);
    }

    public void Stop()
    {
        if (_disposed || _stopped)
        {
            return;
        }

        _stopped = true;
        _queue.CompleteAdding();

        if (_started)
        {
            _thread.Join();
        }
    }

    private void Run()
    {
        Console.WriteLine($"  工作线程启动，ID：{Environment.CurrentManagedThreadId}");

        foreach (OrderJob job in _queue.GetConsumingEnumerable())
        {
            try
            {
                Process(job);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  订单 {job.OrderNo} 处理失败：{ex.Message}");
            }
        }

        Console.WriteLine("  订单队列处理完成，工作线程退出");
    }

    private static void Process(OrderJob job)
    {
        Console.WriteLine($"  开始处理订单：{job.OrderNo}，金额：{job.Amount}");
        Thread.Sleep(300);

        if (job.Amount < 0)
        {
            throw new InvalidOperationException("订单金额不能小于 0");
        }

        Console.WriteLine($"  订单处理完成：{job.OrderNo}");
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Stop();
        _queue.Dispose();
        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(OrderWorker));
        }
    }
}
