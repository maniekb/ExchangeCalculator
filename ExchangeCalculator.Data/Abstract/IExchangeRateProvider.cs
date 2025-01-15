using ExchangeCalculator.Data.Models;

namespace ExchangeCalculator.Data.Abstract;

public interface IExchangeRateProvider
{
    ICollection<ExchangeRate> GetExchangeRates();
}