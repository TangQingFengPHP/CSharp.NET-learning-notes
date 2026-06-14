namespace Thread04_Synchronization;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("=== 04 共享数据同步 ===");
        Console.WriteLine();

        ShowRaceCondition();
        ShowInterlockedFix();
        ShowLockForCompoundOperation();

        Console.WriteLine();
        Console.WriteLine("要点:");
        Console.WriteLine("1. count++ 不是原子操作，多线程会丢更新");
        Console.WriteLine("2. 简单计数用 Interlocked.Increment");
        Console.WriteLine("3. 复合操作（读-判断-写）用 lock 保护临界区");
    }

    private static void ShowRaceCondition()
    {
        Console.WriteLine("--- 竞态条件：count++ ---");

        int count = 0;

        var t1 = new Thread(() => IncrementUnsafe(ref count));
        var t2 = new Thread(() => IncrementUnsafe(ref count));

        t1.Start();
        t2.Start();
        t1.Join();
        t2.Join();

        Console.WriteLine($"  预期 200000，实际 {count}（通常更小）");
        Console.WriteLine();
    }

    private static void ShowInterlockedFix()
    {
        Console.WriteLine("--- Interlocked 修复计数 ---");

        int count = 0;

        var t1 = new Thread(() => IncrementSafe(ref count));
        var t2 = new Thread(() => IncrementSafe(ref count));

        t1.Start();
        t2.Start();
        t1.Join();
        t2.Join();

        Console.WriteLine($"  使用 Interlocked 后：{count}");
        Console.WriteLine();
    }

    private static void ShowLockForCompoundOperation()
    {
        Console.WriteLine("--- lock 保护复合操作 ---");

        var account = new BankAccount(initialBalance: 1000m);

        var t1 = new Thread(() => account.Withdraw(300m));
        var t2 = new Thread(() => account.Withdraw(800m));

        t1.Start();
        t2.Start();
        t1.Join();
        t2.Join();

        Console.WriteLine($"  初始 1000，两线程分别提现 300 和 800，余额={account.Balance}");
        Console.WriteLine();
    }

    private static void IncrementUnsafe(ref int count)
    {
        for (int i = 0; i < 100_000; i++)
        {
            count++;
        }
    }

    private static void IncrementSafe(ref int count)
    {
        for (int i = 0; i < 100_000; i++)
        {
            Interlocked.Increment(ref count);
        }
    }
}

internal sealed class BankAccount
{
    private readonly object _gate = new();
    private decimal _balance;

    public BankAccount(decimal initialBalance)
    {
        _balance = initialBalance;
    }

    public decimal Balance
    {
        get
        {
            lock (_gate)
            {
                return _balance;
            }
        }
    }

    public void Withdraw(decimal amount)
    {
        lock (_gate)
        {
            if (_balance < amount)
            {
                Console.WriteLine($"  余额不足，拒绝提现 {amount}");
                return;
            }

            _balance -= amount;
            Console.WriteLine($"  提现 {amount} 成功，剩余 {_balance}");
        }
    }
}
