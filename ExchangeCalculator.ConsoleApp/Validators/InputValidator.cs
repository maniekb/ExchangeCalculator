using System.Text.RegularExpressions;
using ExchangeCalculator.Data.Models;
using ExchangeCalculator.Data.Models.Enums;
using ErrorMessages = ExchangeCalculator.Data.Models.Constants.ErrorMessages;
    
namespace ExchangeCalculator.ConsoleApp.Validators;

public static class InputValidator
{
    private const string INPUT_REGEX = "Exchange ([A-Za-z]{3})/([A-Za-z]{3}) (-?\\d+)";

    public static ValidationResult Validate(string input)
    {
        if(string.IsNullOrEmpty(input))
            return ValidationResult.WithError(ErrorMessages.InvalidInputErrorMessage);
        
        Match match = Regex.Match(input, INPUT_REGEX, RegexOptions.IgnoreCase);
        
        if(!match.Success)
            return ValidationResult.WithError(ErrorMessages.InvalidInputErrorMessage);

        var mainCurrency = match.Groups[1].Value.ToUpper();
        var moneyCurrency = match.Groups[2].Value.ToUpper();
        var amount = decimal.Parse(match.Groups[3].Value);
        
        if(!Enum.IsDefined(typeof(CurrencyIsoCodes), mainCurrency))
            return ValidationResult.WithError(string.Format(ErrorMessages.InvalidCurrencyIsoCodeErrorMessage, mainCurrency));
        
        if(!Enum.IsDefined(typeof(CurrencyIsoCodes), moneyCurrency))
            return ValidationResult.WithError(string.Format(ErrorMessages.InvalidCurrencyIsoCodeErrorMessage, moneyCurrency));
        
        if (amount < 0)
            return ValidationResult.WithError(ErrorMessages.NegativeNumberErrorMessage);
        
        return ValidationResult.Success(Enum.Parse<CurrencyIsoCodes>(mainCurrency), Enum.Parse<CurrencyIsoCodes>(moneyCurrency), amount);
    }
}