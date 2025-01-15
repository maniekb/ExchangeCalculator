using ExchangeCalculator.Data.Abstract;
using Microsoft.Extensions.Caching.Memory;

namespace ExchangeCalculator.Data.Business;

public class InMemoryCacheService : ICachingService
{
    private readonly IMemoryCache _memoryCache;

    public InMemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public T? Get<T>(string key)
    {
        _memoryCache.TryGetValue(key, out T cacheValue);

        return cacheValue;
    }
    
    public void Set<T>(string key, T value)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(60));

        _memoryCache.Set(key, value, cacheEntryOptions);
    }
}