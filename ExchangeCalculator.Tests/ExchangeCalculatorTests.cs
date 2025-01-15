using ExchangeCalculator.Data.Abstract;
using ExchangeCalculator.Data.Models;
using ExchangeCalculator.Data.Models.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace ExchangeCalculator.Tests;

[TestFixture]
public class ExchangeCalculatorTests
{
    private Mock<IExchangeRateProvider> _exchangeRateProviderMock;
    private Mock<ICachingService> _cachingServiceMock;
    private Mock<ILogger<Data.Business.ExchangeCalculator>> _loggerMock;

    private Data.Business.ExchangeCalculator _sut;

    [SetUp]
    public void SetUp()
    {
        _exchangeRateProviderMock = new Mock<IExchangeRateProvider>();
        _cachingServiceMock = new Mock<ICachingService>();
        _loggerMock = new Mock<ILogger<Data.Business.ExchangeCalculator>>();
        _sut = new Data.Business.ExchangeCalculator(_exchangeRateProviderMock.Object, _cachingServiceMock.Object, _loggerMock.Object);
    }

    [Test]
    public void ShouldReturnError_IfAmountIsNegative()
    {
        var calculationResult = _sut.Calculate(CurrencyIsoCodes.CHF, CurrencyIsoCodes.DKK, -1);
        
        Assert.That(calculationResult.IsSuccess, Is.EqualTo(false));
    }
    
    [Test]
    public void ShouldReturnInputAmount_And_DoNotImportExchangeRates_IfMainAndMoneyCurrencyAreEqual()
    {
        var calculationResult = _sut.Calculate(CurrencyIsoCodes.DKK, CurrencyIsoCodes.DKK, 100);
        
        Assert.That(calculationResult.IsSuccess, Is.EqualTo(true));
        Assert.That(calculationResult.Result, Is.EqualTo(100));
        _exchangeRateProviderMock.Verify(v => v.GetExchangeRates(), Times.Never);
        _cachingServiceMock.Verify(v => v.Get<ICollection<ExchangeRate>>(It.IsAny<string>()), Times.Never);
    }
    
    [TestCase(CurrencyIsoCodes.DKK, CurrencyIsoCodes.EUR, 100, 13.44)]
    [TestCase(CurrencyIsoCodes.EUR, CurrencyIsoCodes.DKK, 100, 743.94)]
    [TestCase(CurrencyIsoCodes.EUR, CurrencyIsoCodes.NOK, 100, 948.90)]
    public void ShouldReturnCalculatedAmount_IfExchangeRateIsFound(CurrencyIsoCodes mainCurrency, CurrencyIsoCodes moneyCurrency, decimal inputAmount, decimal expectedCalculatedAmount)
    {
        _cachingServiceMock.Setup(s => s.Get<ICollection<ExchangeRate>>(It.IsAny<string>()))
            .Returns(new List<ExchangeRate>());
        _exchangeRateProviderMock.Setup(v => v.GetExchangeRates()).Returns(GetExchangeRates);
        
        var calculationResult = _sut.Calculate(mainCurrency, moneyCurrency, inputAmount);
        
        Assert.That(calculationResult.IsSuccess, Is.EqualTo(true));
        Assert.That(Decimal.Round(calculationResult.Result, 2), Is.EqualTo(Decimal.Round(expectedCalculatedAmount, 2)));
        _exchangeRateProviderMock.Verify(v => v.GetExchangeRates(), Times.Once);
        _cachingServiceMock.Verify(v => v.Get<ICollection<ExchangeRate>>(It.IsAny<string>()), Times.Once);
    }
    
    [Test]
    public void ShouldReturnCalculatedAmount_And_NotCallExchangeRateProvider_IfExchangeRatesAreCached()
    {
        var exchangeRates = GetExchangeRates();
        _cachingServiceMock.Setup(s => s.Get<ICollection<ExchangeRate>>(It.IsAny<string>()))
            .Returns(exchangeRates);
        
        var calculationResult = _sut.Calculate(CurrencyIsoCodes.EUR, CurrencyIsoCodes.DKK, 1);
        
        Assert.That(calculationResult.IsSuccess, Is.EqualTo(true));
        _exchangeRateProviderMock.Verify(v => v.GetExchangeRates(), Times.Never);
        _cachingServiceMock.Verify(v => v.Get<ICollection<ExchangeRate>>(It.IsAny<string>()), Times.Once);
    }
    
    [Test]
    public void ShouldReturnError_IfExchangeRateNotAvailable()
    {
        var exchangeRates = GetExchangeRates();
        _cachingServiceMock.Setup(s => s.Get<ICollection<ExchangeRate>>(It.IsAny<string>()))
            .Returns(exchangeRates);
        
        var calculationResult = _sut.Calculate(CurrencyIsoCodes.PLN, CurrencyIsoCodes.DKK, 1);
        
        Assert.That(calculationResult.IsSuccess, Is.EqualTo(false));
        Assert.That(calculationResult.Error, Does.Contain(string.Format(Constants.ErrorMessages.ExchangeRateMissingErrorMessage, CurrencyIsoCodes.PLN, CurrencyIsoCodes.DKK)));
        _cachingServiceMock.Verify(v => v.Get<ICollection<ExchangeRate>>(It.IsAny<string>()), Times.Once);
    }
    
    private ICollection<ExchangeRate> GetExchangeRates()
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