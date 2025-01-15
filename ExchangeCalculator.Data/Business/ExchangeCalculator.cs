using ExchangeCalculator.Data.Abstract;
using ExchangeCalculator.Data.Models;
using ExchangeCalculator.Data.Models.Enums;
using Microsoft.Extensions.Logging;

namespace ExchangeCalculator.Data.Business;

public class ExchangeCalculator : IExchangeCalculator
{
    private const string EXCHANGE_RATES_CACHEKEY = "ExchangeRates";
    private readonly ILogger<ExchangeCalculator> _logger;
    private readonly IExchangeRateProvider _exchangeRateProvider;
    private readonly ICachingService _cachingService;

    public ExchangeCalculator(IExchangeRateProvider exchangeRateProvider, ICachingService cachingService, ILogger<ExchangeCalculator> logger)
    {
        _exchangeRateProvider = exchangeRateProvider;
        _cachingService = cachingService;
        _logger = logger;
    }

    public CalculationResult Calculate(CurrencyIsoCodes mainCurrency, CurrencyIsoCodes moneyCurrency, decimal amount)
    {
        try
        {
            if (amount < 0)
                return CalculationResult.WithError($"[{nameof(ExchangeCalculator)}] Amount cannot be negative.");

            if (mainCurrency == moneyCurrency)
                return CalculationResult.Success(amount);
            
            var exchangeRates = GetExchangeRates();

            // Direct base currency exchange rate available
            var exchangeRate =
                exchangeRates.FirstOrDefault(x => x.Base == mainCurrency && x.Rates.ContainsKey(moneyCurrency));
            if (exchangeRate is not null)
                return CalculationResult.Success(1 / exchangeRate.Rates[moneyCurrency] * amount);

            // Target currency exchange rate available
            exchangeRate =
                exchangeRates.FirstOrDefault(x => x.Base == moneyCurrency && x.Rates.ContainsKey(mainCurrency));
            if (exchangeRate is not null)
                return CalculationResult.Success(exchangeRate.Rates[mainCurrency] * amount);

            // No base/target currency exchange rate available -> use any other possible
            exchangeRate = exchangeRates.FirstOrDefault(x =>
                x.Rates.ContainsKey(mainCurrency) && x.Rates.ContainsKey(moneyCurrency));
            if (exchangeRate is not null)
                return CalculationResult.Success(exchangeRate.Rates[mainCurrency] / exchangeRate.Rates[moneyCurrency] * amount);

            return CalculationResult.WithError(
                $"[{nameof(ExchangeCalculator)}] { String.Format(Constants.ErrorMessages.ExchangeRateMissingErrorMessage, mainCurrency, moneyCurrency) }");
        }
        catch (DivideByZeroException)
        {
            return CalculationResult.WithError(
                $"[{nameof(ExchangeCalculator)}] { String.Format(Constants.ErrorMessages.InvalidExchangeRateErrorMessage, mainCurrency, moneyCurrency) }");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return CalculationResult.WithError(
                $"[{nameof(ExchangeCalculator)}] { Constants.ErrorMessages.DefaultErrorMessage }");
        }
    }

    private ICollection<ExchangeRate> GetExchangeRates()
    {
        var exchangeRates = _cachingService.Get<ICollection<ExchangeRate>>(EXCHANGE_RATES_CACHEKEY);
        
        if (exchangeRates?.Any() != true)
        {
            exchangeRates = _exchangeRateProvider.GetExchangeRates();
            _cachingService.Set(EXCHANGE_RATES_CACHEKEY, exchangeRates);
        }

        return exchangeRates;
    }
}