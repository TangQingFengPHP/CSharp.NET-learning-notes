using System.Runtime.Caching;

var cache = MemoryCache.Default;
CacheItemPolicy policy = new CacheItemPolicy
{
    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10), // 设置绝对过期时间
    SlidingExpiration = TimeSpan.Zero,           // 设置滑动过期时间
    Priority = CacheItemPriority.Default,                  // 设置优先级
    RemovedCallback = args => { Console.WriteLine("缓存项已移除"); } // 过期时执行回调
};

cache.Add("key1", "value1", policy);

Console.WriteLine(cache.Get("key1")); // 输出：value1