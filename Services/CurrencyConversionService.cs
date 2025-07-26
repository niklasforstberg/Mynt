using Microsoft.EntityFrameworkCore;
using Mynt.Data;
using Mynt.Models;

namespace Mynt.Services;

public interface ICurrencyConversionService
{
    Task<decimal?> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency);
}

public class CurrencyConversionService : ICurrencyConversionService
{
    private readonly ApplicationDbContext _db;

    public CurrencyConversionService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<decimal?> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency)
    {
        if (fromCurrency.ToUpper() == toCurrency.ToUpper())
            return amount;

        // Try direct rate
        var directRate = await _db.CurrencyExchangeRates
            .Where(r => r.FromCurrencyCode == fromCurrency.ToUpper()
                       && r.ToCurrencyCode == toCurrency.ToUpper()
                       && r.EffectiveTo == null)
            .OrderByDescending(r => r.EffectiveFrom)
            .FirstOrDefaultAsync();

        if (directRate != null)
            return amount * directRate.Rate;

        // Try reverse rate
        var reverseRate = await _db.CurrencyExchangeRates
            .Where(r => r.FromCurrencyCode == toCurrency.ToUpper()
                       && r.ToCurrencyCode == fromCurrency.ToUpper()
                       && r.EffectiveTo == null)
            .OrderByDescending(r => r.EffectiveFrom)
            .FirstOrDefaultAsync();

        if (reverseRate != null)
            return amount / reverseRate.Rate;

        // Cross-currency conversion using EUR as base
        var eurToFromRate = await _db.CurrencyExchangeRates
            .Where(r => r.FromCurrencyCode == "EUR"
                       && r.ToCurrencyCode == fromCurrency.ToUpper()
                       && r.EffectiveTo == null)
            .OrderByDescending(r => r.EffectiveFrom)
            .FirstOrDefaultAsync();

        var eurToToRate = await _db.CurrencyExchangeRates
            .Where(r => r.FromCurrencyCode == "EUR"
                       && r.ToCurrencyCode == toCurrency.ToUpper()
                       && r.EffectiveTo == null)
            .OrderByDescending(r => r.EffectiveFrom)
            .FirstOrDefaultAsync();

        if (eurToFromRate != null && eurToToRate != null)
        {
            var crossRate = eurToToRate.Rate / eurToFromRate.Rate;
            return amount * crossRate;
        }

        return null; // No conversion rate available
    }
}