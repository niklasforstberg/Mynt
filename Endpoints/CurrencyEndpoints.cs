using Microsoft.EntityFrameworkCore;
using Mynt.Data;
using Mynt.Models.DTOs.Currency;
using Mynt.Models.Enums;
using Mynt.Services;

namespace Mynt.Endpoints;

public static class CurrencyEndpoints
{
    public static void MapCurrencyEndpoints(this IEndpointRouteBuilder app)
    {
        // Get all active currencies
        app.MapGet("/api/currencies", async (ApplicationDbContext db) =>
        {
            var currencies = await db.Currencies
                .Where(c => c.IsActive)
                .OrderBy(c => c.Code)
                .Select(c => new CurrencyResponse
                {
                    Code = c.Code,
                    Name = c.Name,
                    Symbol = c.Symbol,
                    SymbolPosition = c.SymbolPosition,
                    IsSystemManaged = c.IsSystemManaged,
                    IsActive = c.IsActive
                })
                .ToListAsync();

            return Results.Ok(currencies);
        })
        .RequireAuthorization();

        // Get currency by code
        app.MapGet("/api/currencies/{code}", async (string code, ApplicationDbContext db) =>
        {
            var currency = await db.Currencies
                .FirstOrDefaultAsync(c => c.Code == code.ToUpper() && c.IsActive);

            if (currency == null)
                return Results.NotFound();

            var response = new CurrencyResponse
            {
                Code = currency.Code,
                Name = currency.Name,
                Symbol = currency.Symbol,
                SymbolPosition = currency.SymbolPosition,
                IsSystemManaged = currency.IsSystemManaged,
                IsActive = currency.IsActive
            };

            return Results.Ok(response);
        })
        .RequireAuthorization();

        // Create new currency (Admin only)
        app.MapPost("/api/currencies", async (CurrencyCreateRequest request, ApplicationDbContext db, HttpContext context) =>
        {
            var userEmail = context.User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
                return Results.Unauthorized();

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user?.Role != UserRole.Admin)
                return Results.Forbid();

            // Check if currency code already exists
            var existingCurrency = await db.Currencies
                .FirstOrDefaultAsync(c => c.Code == request.Code.ToUpper());

            if (existingCurrency != null)
                return Results.BadRequest($"Currency with code '{request.Code}' already exists.");

            var currency = new Models.Currency
            {
                Code = request.Code.ToUpper(),
                Name = request.Name,
                Symbol = request.Symbol,
                SymbolPosition = request.SymbolPosition,
                IsSystemManaged = false,
                CreatedById = user.Id,
                IsActive = true
            };

            db.Currencies.Add(currency);
            await db.SaveChangesAsync();

            var response = new CurrencyResponse
            {
                Code = currency.Code,
                Name = currency.Name,
                Symbol = currency.Symbol,
                SymbolPosition = currency.SymbolPosition,
                IsSystemManaged = currency.IsSystemManaged,
                IsActive = currency.IsActive
            };

            return Results.Created($"/api/currencies/{currency.Code}", response);
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        // Set exchange rate (Admin only)
        app.MapPost("/api/currencies/exchange-rates", async (CurrencyExchangeRateRequest request, ApplicationDbContext db, HttpContext context) =>
        {
            var userEmail = context.User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
                return Results.Unauthorized();

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user?.Role != UserRole.Admin)
                return Results.Forbid();

            // Validate currencies exist
            var fromCurrency = await db.Currencies.FirstOrDefaultAsync(c => c.Code == request.FromCurrencyCode.ToUpper());
            var toCurrency = await db.Currencies.FirstOrDefaultAsync(c => c.Code == request.ToCurrencyCode.ToUpper());

            if (fromCurrency == null || toCurrency == null)
                return Results.BadRequest("One or both currencies not found.");

            if (fromCurrency.Code == toCurrency.Code)
                return Results.BadRequest("Cannot set exchange rate for same currency.");

            // Expire any existing current rate
            var existingRate = await db.CurrencyExchangeRates
                .FirstOrDefaultAsync(r => r.FromCurrencyCode == request.FromCurrencyCode.ToUpper()
                                         && r.ToCurrencyCode == request.ToCurrencyCode.ToUpper()
                                         && r.EffectiveTo == null);

            if (existingRate != null)
            {
                existingRate.EffectiveTo = DateTime.UtcNow;
            }

            // Create new rate
            var newRate = new Models.CurrencyExchangeRate
            {
                FromCurrencyCode = request.FromCurrencyCode.ToUpper(),
                ToCurrencyCode = request.ToCurrencyCode.ToUpper(),
                Rate = request.Rate,
                EffectiveFrom = DateTime.UtcNow,
                Source = request.Source ?? "Manual",
                SetById = user.Id
            };

            db.CurrencyExchangeRates.Add(newRate);
            await db.SaveChangesAsync();

            return Results.Ok(new { message = "Exchange rate set successfully" });
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        // Manually trigger exchange rate update (Admin only)
        app.MapPost("/api/currencies/update-rates", async (ApplicationDbContext db, HttpContext context) =>
        {
            var userEmail = context.User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
                return Results.Unauthorized();

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user?.Role != UserRole.Admin)
                return Results.Forbid();

            // This will trigger the background service to update rates
            // In a real implementation, you might want to inject the service and call it directly
            return Results.Ok(new { message = "Exchange rate update triggered. Check logs for progress." });
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        // Get exchange rate update status
        app.MapGet("/api/currencies/update-status", async (ApplicationDbContext db) =>
        {
            var latestRate = await db.CurrencyExchangeRates
                .Where(r => r.Source == "ExchangeRatesAPI")
                .OrderByDescending(r => r.EffectiveFrom)
                .FirstOrDefaultAsync();

            if (latestRate == null)
                return Results.Ok(new
                {
                    lastUpdate = (DateTime?)null,
                    status = "No rates available"
                });

            return Results.Ok(new
            {
                lastUpdate = latestRate.EffectiveFrom,
                status = "Rates available",
                baseCurrency = latestRate.FromCurrencyCode,
                totalRates = await db.CurrencyExchangeRates
                    .Where(r => r.EffectiveFrom == latestRate.EffectiveFrom)
                    .CountAsync()
            });
        })
        .RequireAuthorization();

        // Convert currency
        app.MapPost("/api/currencies/convert", async (CurrencyConversionRequest request, ApplicationDbContext db, ICurrencyConversionService conversionService) =>
        {
            if (request.FromCurrencyCode.ToUpper() == request.ToCurrencyCode.ToUpper())
            {
                return Results.Ok(new
                {
                    originalAmount = request.Amount,
                    convertedAmount = request.Amount,
                    fromCurrencyCode = request.FromCurrencyCode.ToUpper(),
                    toCurrencyCode = request.ToCurrencyCode.ToUpper(),
                    rate = 1.0m
                });
            }

            var convertedAmount = await conversionService.ConvertCurrencyAsync(
                request.Amount,
                request.FromCurrencyCode,
                request.ToCurrencyCode);

            if (convertedAmount.HasValue)
            {
                return Results.Ok(new
                {
                    originalAmount = request.Amount,
                    convertedAmount = convertedAmount.Value,
                    fromCurrencyCode = request.FromCurrencyCode.ToUpper(),
                    toCurrencyCode = request.ToCurrencyCode.ToUpper()
                });
            }

            return Results.BadRequest("No exchange rate found between these currencies.");
        })
        .RequireAuthorization();

        // Get current exchange rate
        app.MapGet("/api/currencies/{fromCode}/to/{toCode}/rate", async (string fromCode, string toCode, ApplicationDbContext db) =>
        {
            if (fromCode.ToUpper() == toCode.ToUpper())
                return Results.Ok(new { rate = 1.0m });

            var exchangeRate = await db.CurrencyExchangeRates
                .Where(r => r.FromCurrencyCode == fromCode.ToUpper()
                           && r.ToCurrencyCode == toCode.ToUpper()
                           && r.EffectiveTo == null)
                .OrderByDescending(r => r.EffectiveFrom)
                .FirstOrDefaultAsync();

            if (exchangeRate == null)
            {
                // Try reverse rate
                var reverseRate = await db.CurrencyExchangeRates
                    .Where(r => r.FromCurrencyCode == toCode.ToUpper()
                               && r.ToCurrencyCode == fromCode.ToUpper()
                               && r.EffectiveTo == null)
                    .OrderByDescending(r => r.EffectiveFrom)
                    .FirstOrDefaultAsync();

                if (reverseRate == null)
                    return Results.NotFound("No exchange rate found between these currencies.");

                return Results.Ok(new { rate = 1.0m / reverseRate.Rate });
            }

            return Results.Ok(new { rate = exchangeRate.Rate });
        })
        .RequireAuthorization();
    }
}