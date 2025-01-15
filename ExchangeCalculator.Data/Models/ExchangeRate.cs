using ExchangeCalculator.Data.Models.Enums;

namespace ExchangeCalculator.Data.Models;

public class ExchangeRate
{
    public CurrencyIsoCodes Base { get; set; }
    public Dictionary<CurrencyIsoCodes, decimal> Rates { get; set; }
}