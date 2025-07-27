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
        group.MapGet("/", async (ApplicationDbContext db, HttpContext context, bool? isAsset) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            Console.WriteLine($"User ID: {userId}");
            Console.WriteLine($"Assets: {db.Assets.Count()}");

            var query = db.Assets
                .Include(a => a.AssetType)
                .Include(a => a.FinancialGroup)
                .Include(a => a.AssetValues.OrderByDescending(av => av.RecordedAt).Take(1))
                .Where(a => a.UserId == userId);

            // Filter by asset type if specified
            if (isAsset.HasValue)
                query = query.Where(a => a.AssetType != null && a.AssetType.IsAsset == isAsset.Value);

            var assets = await query.ToListAsync();

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
                .Include(a => a.AssetType)
                .Include(a => a.AssetValues.OrderByDescending(av => av.RecordedAt).Take(1))
                .Where(a => a.UserId == userId)
                .ToListAsync();

            var assetSummary = 0m;
            var assetCount = 0;
            var debtSummary = 0m;
            var debtCount = 0;
            var lastUpdated = assets
                .SelectMany(a => a.AssetValues)
                .OrderByDescending(av => av.RecordedAt)
                .FirstOrDefault()?.RecordedAt;

            foreach (var asset in assets)
            {
                var latestValue = asset.AssetValues.FirstOrDefault();
                if (latestValue != null)
                {
                    var assetCurrency = asset.CurrencyCode ?? preferredCurrency;
                    var convertedValue = await conversionService.ConvertCurrencyAsync(
                        latestValue.Value,
                        assetCurrency,
                        preferredCurrency);

                    if (convertedValue.HasValue)
                    {
                        // Determine if this is an asset or debt based on AssetType
                        var isAsset = asset.AssetType?.IsAsset ?? true; // Default to asset if no type

                        if (isAsset)
                        {
                            assetSummary += convertedValue.Value;
                            assetCount++;
                        }
                        else
                        {
                            debtSummary += convertedValue.Value;
                            debtCount++;
                        }
                    }
                }
                else
                {
                    // Count assets without values
                    var isAsset = asset.AssetType?.IsAsset ?? true;
                    if (isAsset)
                        assetCount++;
                    else
                        debtCount++;
                }
            }

            var totalSummary = assetSummary - debtSummary;

            var summary = new AssetSummaryResponse
            {
                AssetSummary = assetSummary,
                AssetCount = assetCount,
                DebtSummary = debtSummary,
                DebtCount = debtCount,
                TotalSummary = totalSummary,
                LastUpdated = lastUpdated,
                CurrencyCode = preferredCurrency
            };

            return Results.Ok(summary);
        });

        // GET: Get historical snapshots for the current user
        group.MapGet("/snapshots", async (ISnapshotService snapshotService, HttpContext context, DateTime? fromDate, DateTime? toDate) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var snapshots = await snapshotService.GetUserSnapshotsAsync(userId, fromDate, toDate);
            return Results.Ok(snapshots);
        });


    }
}