// 生成指定范围的随机整数
Random random = new Random();
int randomInt = random.Next(1, 100);  // 生成 1 到 100 之间的整数
Console.WriteLine(randomInt);

// 生成随机布尔值
bool randomBool = random.Next(2) == 0;  // 随机生成 true 或 false
Console.WriteLine(randomBool);

// 生成随机字节数组
byte[] bytes = new byte[10];
random.NextBytes(bytes);  // 填充随机字节
Console.WriteLine(string.Join(", ", bytes));

// 生成随机浮点数
double randomDouble = random.NextDouble();  // 生成 [0.0, 1.0) 范围内的随机浮点数
Console.WriteLine(randomDouble);

// 线程安全的随机数生成
// 使用 ThreadLocal 来确保每个线程都有自己的 Random 实例
ThreadLocal<Random> threadRandom = new ThreadLocal<Random>(() => new Random());
if (threadRandom.Value != null)
{
    int safeRandomInt = threadRandom.Value.Next(1, 100);  // 每个线程都有独立的随机数生成器
    Console.WriteLine(safeRandomInt);
}

// 使用 Random.Shared
int number = Random.Shared.Next(1, 100); // 线程安全 
Console.WriteLine(number);