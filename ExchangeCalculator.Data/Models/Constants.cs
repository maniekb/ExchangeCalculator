namespace ExchangeCalculator.Data.Models;

public static class Constants
{
    public static class ErrorMessages
    {
        public const string InvalidInputErrorMessage = "Invalid input. Should follow pattern: 'Exchange DKK/EUR 1'.";
        public const string NegativeNumberErrorMessage = "Invalid amount. Should be positive number.";
        public const string InvalidCurrencyIsoCodeErrorMessage = "Currency {0} is not a valid ISO currency code.";
        public const string ExchangeRateMissingErrorMessage = "Exchange rate is missing for pair {0}/{1}";
        public const string InvalidExchangeRateErrorMessage = "Invalid exchange rate for pair {0}/{1}.";
        public const string DefaultErrorMessage = "Something went wrong. Please try again.";
    }
}