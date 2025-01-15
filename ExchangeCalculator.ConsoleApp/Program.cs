using ExchangeCalculator.ConsoleApp.Validators;
using ExchangeCalculator.Data.Abstract;
using ExchangeCalculator.Data.Business;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddTransient<IExchangeCalculator, ExchangeCalculator.Data.Business.ExchangeCalculator>();
builder.Services.AddTransient<IExchangeRateProvider, InMemoryExchangeRateProvider>();
builder.Services.AddTransient<ICachingService, InMemoryCacheService>();

using IHost host = builder.Build();

Console.WriteLine("Usage: Exchange <currency pair> <amount to exchange>");

string input = Console.ReadLine();
var validationResult = InputValidator.Validate(input);
if (!validationResult.IsValid)
{
    Console.WriteLine(validationResult.Error);
    return;
}

var calculator = host.Services.GetService<IExchangeCalculator>() ?? throw new Exception("No service found");
var calculationResult = calculator.Calculate(validationResult.MainCurrency, validationResult.MoneyCurrency, validationResult.Amount);

if(calculationResult.IsSuccess)
    Console.WriteLine(calculationResult.Result);
else
{
    Console.WriteLine(calculationResult.Error);
}