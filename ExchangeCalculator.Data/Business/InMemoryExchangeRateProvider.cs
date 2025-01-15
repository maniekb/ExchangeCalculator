using ExchangeCalculator.Data.Abstract;
using ExchangeCalculator.Data.Models;
using ExchangeCalculator.Data.Models.Enums;

namespace ExchangeCalculator.Data.Business;

public class InMemoryExchangeRateProvider : IExchangeRateProvider
{
    public ICollection<ExchangeRate> GetExchangeRates()
    {
        return new List<ExchangeRate>()
        {
            new ()
            {
                Base = CurrencyIsoCodes.DKK,
                Rates = new Dictionary<CurrencyIsoCodes, decimal>()
                {
                    { CurrencyIsoCodes.EUR, 7.4394m },
                    { CurrencyIsoCodes.USD, 6.6311m },
                    { CurrencyIsoCodes.GBP, 8.5285m },
                    { CurrencyIsoCodes.SEK, 0.7610m },
                    { CurrencyIsoCodes.NOK, 0.7840m },
                    { CurrencyIsoCodes.CHF, 6.8358m },
                    { CurrencyIsoCodes.JPY, 0.05974m }
                }
            }
        };
    }
}