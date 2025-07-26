using Microsoft.EntityFrameworkCore;
using Mynt.Data;
using Mynt.Models;
using Mynt.Models.DTOs.Asset;
using System.Security.Claims;
using Mynt.Services;

namespace Mynt.Endpoints;

public static class AssetEndpoints
{
    public static void MapAssetEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/assets").RequireAuthorization();

        // CREATE: Create a new asset
        group.MapPost("/", async (AssetCreateRequest request, ApplicationDbContext db, HttpContext context) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var asset = new Asset
            {
                Name = request.Name,
                Description = request.Description,
                UserId = userId,
                FinancialGroupId = request.FinancialGroupId,
                AssetTypeId = request.AssetTypeId,
                CreatedAt = DateTime.UtcNow,
                CurrencyCode = request.CurrencyCode
            };

            db.Assets.Add(asset);
            await db.SaveChangesAsync();

            // Create initial value record if provided
            if (request.InitialValue.HasValue)
            {
                var assetValue = new AssetValue
                {
                    AssetId = asset.Id,
                    Value = request.InitialValue.Value,
                    RecordedAt = DateTime.UtcNow,
                    Asset = asset
                };
                db.AssetValues.Add(assetValue);
                await db.SaveChangesAsync();
            }

            var response = new AssetResponse
            {
                Id = asset.Id,
                Name = asset.Name ?? "",
                Description = asset.Description,
                FinancialGroupId = asset.FinancialGroupId,
                FinancialGroupName = null, // Will be populated if needed
                AssetTypeId = asset.AssetTypeId,
                AssetTypeName = null, // Will be populated if needed
                CreatedAt = asset.CreatedAt,
                CurrentValue = request.InitialValue,
                CurrencyCode = asset.CurrencyCode
            };

            return Results.Created($"/api/assets/{asset.Id}", response);
        });

        // READ: Get all assets for the current user
        group.MapGet("/", async (ApplicationDbContext db, HttpContext context) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            Console.WriteLine($"User ID: {userId}");
            Console.WriteLine($"Assets: {db.Assets.Count()}");
            var assets = await db.Assets
                .Include(a => a.AssetType)
                .Include(a => a.FinancialGroup)
                .Include(a => a.AssetValues.OrderByDescending(av => av.RecordedAt).Take(1))
                .Where(a => a.UserId == userId)
                .ToListAsync();

            var response = assets.Select(a => new AssetResponse
            {
                Id = a.Id,
                Name = a.Name ?? "",
                Description = a.Description,
                FinancialGroupId = a.FinancialGroupId,
                FinancialGroupName = a.FinancialGroup?.Name,
                AssetTypeId = a.AssetTypeId,
                AssetTypeName = a.AssetType?.Translations.FirstOrDefault(t => t.LanguageCode == "en")?.Name,
                CreatedAt = a.CreatedAt,
                CurrentValue = a.AssetValues.OrderByDescending(av => av.RecordedAt).FirstOrDefault()?.Value,
                CurrencyCode = a.CurrencyCode
            });

            return Results.Ok(response);
        });

        // READ: Get a specific asset by ID
        group.MapGet("/{id}", async (int id, ApplicationDbContext db, HttpContext context) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var asset = await db.Assets
                .Include(a => a.AssetType)
                .Include(a => a.FinancialGroup)
                .Include(a => a.AssetValues.OrderByDescending(av => av.RecordedAt).Take(1))
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (asset == null)
                return Results.NotFound();

            var response = new AssetResponse
            {
                Id = asset.Id,
                Name = asset.Name ?? "",
                Description = asset.Description,
                FinancialGroupId = asset.FinancialGroupId,
                FinancialGroupName = asset.FinancialGroup?.Name,
                AssetTypeId = asset.AssetTypeId,
                AssetTypeName = asset.AssetType?.Translations.FirstOrDefault(t => t.LanguageCode == "en")?.Name,
                CreatedAt = asset.CreatedAt,
                CurrentValue = asset.AssetValues.OrderByDescending(av => av.RecordedAt).FirstOrDefault()?.Value,
                CurrencyCode = asset.CurrencyCode
            };

            return Results.Ok(response);
        });

        // UPDATE: Update an existing asset
        group.MapPut("/{id}", async (int id, AssetUpdateRequest request, ApplicationDbContext db, HttpContext context) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var asset = await db.Assets.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            if (asset == null)
                return Results.NotFound();

            asset.Name = request.Name;
            asset.Description = request.Description;
            asset.FinancialGroupId = request.FinancialGroupId;
            asset.AssetTypeId = request.AssetTypeId;
            asset.CurrencyCode = request.CurrencyCode;

            await db.SaveChangesAsync();

            // Reload the asset with includes to populate the response DTO
            var updatedAsset = await db.Assets
                .Include(a => a.AssetType)
                .Include(a => a.FinancialGroup)
                .Include(a => a.AssetValues.OrderByDescending(av => av.RecordedAt).Take(1))
                .FirstOrDefaultAsync(a => a.Id == id);

            var response = new AssetResponse
            {
                Id = updatedAsset!.Id,
                Name = updatedAsset.Name ?? "",
                Description = updatedAsset.Description,
                FinancialGroupId = updatedAsset.FinancialGroupId,
                FinancialGroupName = updatedAsset.FinancialGroup?.Name,
                AssetTypeId = updatedAsset.AssetTypeId,
                AssetTypeName = updatedAsset.AssetType?.Translations.FirstOrDefault(t => t.LanguageCode == "en")?.Name,
                CreatedAt = updatedAsset.CreatedAt,
                CurrentValue = updatedAsset.AssetValues.OrderByDescending(av => av.RecordedAt).FirstOrDefault()?.Value,
                CurrencyCode = updatedAsset.CurrencyCode
            };

            return Results.Ok(response);
        });

        // DELETE: Delete an asset
        group.MapDelete("/{id}", async (int id, ApplicationDbContext db, HttpContext context) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var asset = await db.Assets.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            if (asset == null)
                return Results.NotFound();

            db.Assets.Remove(asset);
            await db.SaveChangesAsync();
            return Results.Ok();
        });

        // GET: Get asset summary for the current user
        group.MapGet("/summary", async (ApplicationDbContext db, HttpContext context, ICurrencyConversionService conversionService) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            // Get user's preferred currency
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var preferredCurrency = user?.GetPreferredCurrency() ?? "USD";

            var assets = await db.Assets
                .Include(a => a.AssetValues.OrderByDescending(av => av.RecordedAt).Take(1))
                .Where(a => a.UserId == userId)
                .ToListAsync();

            var totalValue = 0m;
            var assetsWithValues = 0;

            foreach (var asset in assets)
            {
                var latestValue = asset.AssetValues.FirstOrDefault();
                if (latestValue != null)
                {
                    assetsWithValues++;

                    var assetCurrency = asset.CurrencyCode ?? "USD";
                    var convertedValue = await conversionService.ConvertCurrencyAsync(
                        latestValue.Value,
                        assetCurrency,
                        preferredCurrency);

                    if (convertedValue.HasValue)
                    {
                        totalValue += convertedValue.Value;
                    }
                }
            }

            var summary = new AssetSummaryResponse
            {
                TotalValue = totalValue,
                AssetCount = assets.Count,
                AssetsWithValues = assetsWithValues,
                LastUpdated = assets
                    .SelectMany(a => a.AssetValues)
                    .OrderByDescending(av => av.RecordedAt)
                    .FirstOrDefault()?.RecordedAt,
                PreferredCurrency = preferredCurrency
            };

            return Results.Ok(summary);
        });
    }
}