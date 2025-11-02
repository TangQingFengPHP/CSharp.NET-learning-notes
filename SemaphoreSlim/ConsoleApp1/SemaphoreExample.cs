namespace ConsoleApp1;

public class SemaphoreExample
{
    private static SemaphoreSlim semaphore = new SemaphoreSlim(3, 3);  // 最多允许 3 个线程同时执行

    public static async Task Execute()
    {
        var tasks = new List<Task>();

        for (int i = 0; i < 10; i++)  // 10 个任务，但最多 3 个同时执行
        {
            int taskId = i;
            tasks.Add(Task.Run(async () =>
            {
                await semaphore.WaitAsync();  // 请求信号量
                try
                {
                    Console.WriteLine($"Task {taskId} started.");
                    await Task.Delay(1000);  // 模拟任务执行
                    Console.WriteLine($"Task {taskId} completed.");
                }
                finally
                {
                    semaphore.Release();  // 释放信号量
                }
            }));
        }

        await Task.WhenAll(tasks);  // 等待所有任务完成
    }
}