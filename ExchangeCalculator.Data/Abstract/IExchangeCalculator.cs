using ExchangeCalculator.Data.Models;
using ExchangeCalculator.Data.Models.Enums;

namespace ExchangeCalculator.Data.Abstract;

public interface IExchangeCalculator
{
    CalculationResult Calculate(CurrencyIsoCodes mainCurrency, CurrencyIsoCodes moneyCurrency, decimal amount);
}