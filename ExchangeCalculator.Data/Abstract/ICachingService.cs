namespace ExchangeCalculator.Data.Abstract;

public interface ICachingService
{
    T? Get<T>(string key);
    void Set<T>(string key, T value);
}