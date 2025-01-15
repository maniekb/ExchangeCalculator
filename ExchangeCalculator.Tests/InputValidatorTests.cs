using ExchangeCalculator.ConsoleApp.Validators;
using ExchangeCalculator.Data.Models;
using ExchangeCalculator.Data.Models.Enums;

namespace ExchangeCalculator.Tests;

[TestFixture]
public class Tests
{
    [TestCase("")]
    [TestCase("whoops")]
    [TestCase("DKK/EURR 1")]
    [TestCase("Exchange DKK/EURR 1")]
    [TestCase("Exchange DKK/EURR")]
    [TestCase("Exchangee DKK/EUR 1")]
    public void ShouldReturnError_IfInputDoesNotFollowPattern(string input)
    {
        var validationResult = InputValidator.Validate(input);
        Assert.That(validationResult.IsValid, Is.EqualTo(false));
        Assert.That(validationResult.Error, Is.EqualTo(Constants.ErrorMessages.InvalidInputErrorMessage));
    }
    
    [TestCase("Exchange LOL/EUR 1")]
    [TestCase("Exchange EUR/LOL 1")]
    public void ShouldReturnError_IfInputHasInvalidIsoCurrencyCode(string input)
    {
        var validationResult = InputValidator.Validate(input);
        Assert.That(validationResult.IsValid, Is.EqualTo(false));
        Assert.That(validationResult.Error, Is.EqualTo(string.Format(Constants.ErrorMessages.InvalidCurrencyIsoCodeErrorMessage, "LOL")));
    }
    
    [TestCase("Exchange DKK/EUR -1")]
    [TestCase("Exchange DKK/EUR -199.98")]
    public void ShouldReturnError_IfAmountIsNegative(string input)
    {
        var validationResult = InputValidator.Validate(input);
        Assert.That(validationResult.IsValid, Is.EqualTo(false));
        Assert.That(validationResult.Error, Is.EqualTo(Constants.ErrorMessages.NegativeNumberErrorMessage));
    }
    
    [TestCase("Exchange DKK/EUR 100", CurrencyIsoCodes.DKK, CurrencyIsoCodes.EUR, 100)]
    [TestCase("exchange NOK/USD 200", CurrencyIsoCodes.NOK, CurrencyIsoCodes.USD, 200)]
    [TestCase("exchange dkk/eur 300", CurrencyIsoCodes.DKK, CurrencyIsoCodes.EUR, 300)]
    public void ShouldReturnSuccessWithParsedValues_IfInputIsValid(string input, CurrencyIsoCodes expectedMainCurrency, CurrencyIsoCodes expectedMoneyCurrency, decimal expectedAmount)
    {
        var validationResult = InputValidator.Validate(input);
        Assert.That(validationResult.IsValid, Is.EqualTo(true));
        Assert.That(validationResult.MainCurrency, Is.EqualTo(expectedMainCurrency));
        Assert.That(validationResult.MoneyCurrency, Is.EqualTo(expectedMoneyCurrency));
        Assert.That(validationResult.Amount, Is.EqualTo(expectedAmount));
    }
}