using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mynt.Data;
using Mynt.Models;
using Mynt.Models.DTOs.Currency;
using System.Text.Json;

namespace Mynt.Services;

public class ExchangeRateUpdateService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ExchangeRateUpdateService> _logger;
    private readonly TimeSpan _period = TimeSpan.FromHours(24); // Run daily

    public ExchangeRateUpdateService(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<ExchangeRateUpdateService> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Exchange Rate Update Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateExchangeRatesAsync();
                _logger.LogInformation("Exchange rates updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating exchange rates");
            }

            await Task.Delay(_period, stoppingToken);
        }
    }

    private async Task UpdateExchangeRatesAsync()
    {
        var accessKey = _configuration["ExchangeRatesApi:AccessKey"];
        if (string.IsNullOrEmpty(accessKey))
        {
            _logger.LogError("Exchange Rates API access key not configured");
            return;
        }

        var url = $"http://api.exchangeratesapi.io/v1/latest?access_key={accessKey}&format=1";

        using var httpClient = new HttpClient();
        var response = await httpClient.GetStringAsync(url);
        var apiResponse = JsonSerializer.Deserialize<ExchangeRatesApiResponse>(response);

        if (apiResponse?.Success != true || apiResponse.Rates == null)
        {
            _logger.LogError("Failed to fetch exchange rates from API");
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Get all active currencies from our database
        var currencies = await dbContext.Currencies
            .Where(c => c.IsActive)
            .Select(c => c.Code)
            .ToListAsync();

        var baseCurrency = apiResponse.Base;
        var rates = apiResponse.Rates;
        var effectiveFrom = DateTime.UtcNow;

        // Expire existing rates
        var existingRates = await dbContext.CurrencyExchangeRates
            .Where(r => r.EffectiveTo == null)
            .ToListAsync();

        foreach (var rate in existingRates)
        {
            rate.EffectiveTo = effectiveFrom;
        }

        // Add new rates
        var newRates = new List<CurrencyExchangeRate>();

        foreach (var currency in currencies)
        {
            if (currency == baseCurrency) continue;

            if (rates.TryGetValue(currency, out var rate))
            {
                newRates.Add(new CurrencyExchangeRate
                {
                    FromCurrencyCode = baseCurrency,
                    ToCurrencyCode = currency,
                    Rate = rate,
                    EffectiveFrom = effectiveFrom,
                    Source = "ExchangeRatesAPI",
                    SetById = null // System-managed
                });
            }
        }

        dbContext.CurrencyExchangeRates.AddRange(newRates);
        await dbContext.SaveChangesAsync();

        _logger.LogInformation("Updated {Count} exchange rates from {BaseCurrency}",
            newRates.Count, baseCurrency);
    }
}