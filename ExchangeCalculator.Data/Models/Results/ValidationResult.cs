using System.Collections.ObjectModel;
using ExchangeCalculator.Data.Models.Enums;

namespace ExchangeCalculator.Data.Models;

public class ValidationResult
{
    public CurrencyIsoCodes MainCurrency { get; set; }
    public CurrencyIsoCodes MoneyCurrency { get; set; }
    public decimal Amount { get; set; }
    
    public string Error { get; set; }
    public bool IsValid => string.IsNullOrEmpty(Error);
    
    public static ValidationResult WithError(string error) => new() { Error = error };
 
    public static ValidationResult Success(CurrencyIsoCodes mainCurrency, CurrencyIsoCodes moneyCurrency, decimal amount) 
        => new()
        {
            MainCurrency = mainCurrency,
            MoneyCurrency = moneyCurrency,
            Amount = amount
        };
}