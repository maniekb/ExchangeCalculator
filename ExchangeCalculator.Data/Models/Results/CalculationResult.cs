namespace ExchangeCalculator.Data.Models;

public class CalculationResult
{
    public decimal Result { get; set; }
    public string Error { get; set; }
    public bool IsSuccess => string.IsNullOrEmpty(Error);
    
    public static CalculationResult WithError(string error) => new() { Error = error };
    public static CalculationResult Success(decimal result) => new() { Result = result };
}