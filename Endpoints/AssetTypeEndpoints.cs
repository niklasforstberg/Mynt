using Microsoft.EntityFrameworkCore;
using Mynt.Data;
using Mynt.Models;
using Mynt.Models.DTOs.AssetType;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Mynt.Endpoints;

public static class AssetTypeEndpoints
{
    public static void MapAssetTypeEndpoints(this IEndpointRouteBuilder app)
    {
        // Get all asset types
        app.MapGet("/api/asset-types", async (ApplicationDbContext db, string? lang, bool? isAsset) =>
        {
            var query = db.AssetTypes
                .Include(at => at.Translations)
                .AsQueryable();

            // Filter by asset type if specified
            if (isAsset.HasValue)
                query = query.Where(at => at.IsAsset == isAsset.Value);

            var results = await query.ToListAsync();

            return results.Select(at => new AssetTypeListResponse
            {
                Id = at.Id,
                Name = !string.IsNullOrEmpty(lang)
                    ? at.Translations.FirstOrDefault(t => t.LanguageCode == lang)?.Name ?? at.DefaultName
                    : at.DefaultName,
                IsAsset = at.IsAsset,
                IsPhysical = at.IsPhysical
            });
        });

        // Create a new asset type with translations
        app.MapPost("/api/asset-types", async (AssetTypeCreateRequest request, ApplicationDbContext db) =>
        {
            var assetType = new AssetType
            {
                DefaultName = request.DefaultName,
                IsAsset = request.IsAsset,
                IsPhysical = request.IsPhysical,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add translations
            foreach (var translation in request.Translations)
            {
                assetType.Translations.Add(new AssetTypeTranslation
                {
                    LanguageCode = translation.Key,
                    Name = translation.Value
                });
            }

            db.AssetTypes.Add(assetType);
            await db.SaveChangesAsync();

            var response = new AssetTypeResponse
            {
                Id = assetType.Id,
                DefaultName = assetType.DefaultName,
                IsAsset = assetType.IsAsset,
                IsPhysical = assetType.IsPhysical,
                CreatedAt = assetType.CreatedAt,
                UpdatedAt = assetType.UpdatedAt,
                Translations = assetType.Translations.Select(t => new AssetTypeTranslationResponse
                {
                    Id = t.Id,
                    Name = t.Name,
                    LanguageCode = t.LanguageCode
                }).ToList()
            };

            return Results.Created($"/api/asset-types/{assetType.Id}", response);
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        // Update an existing asset type
        app.MapPut("/api/asset-types/{id}", async (int id, AssetTypeUpdateRequest request, ApplicationDbContext db) =>
        {
            var assetType = await db.AssetTypes
                .Include(at => at.Translations)
                .FirstOrDefaultAsync(at => at.Id == id);

            if (assetType == null)
                return Results.NotFound();

            // Update basic properties if provided
            if (!string.IsNullOrEmpty(request.DefaultName))
                assetType.DefaultName = request.DefaultName;

            if (request.IsAsset.HasValue)
                assetType.IsAsset = request.IsAsset.Value;

            if (request.IsPhysical.HasValue)
                assetType.IsPhysical = request.IsPhysical.Value;

            // Update timestamp
            assetType.UpdatedAt = DateTime.UtcNow;

            // Update translations if provided
            if (request.Translations.Any())
            {
                // Remove existing translations that are being updated
                var translationsToUpdate = assetType.Translations
                    .Where(t => request.Translations.ContainsKey(t.LanguageCode))
                    .ToList();

                foreach (var translation in translationsToUpdate)
                {
                    translation.Name = request.Translations[translation.LanguageCode];
                }

                // Add new translations
                var existingLanguageCodes = assetType.Translations.Select(t => t.LanguageCode).ToHashSet();
                var newTranslations = request.Translations
                    .Where(kvp => !existingLanguageCodes.Contains(kvp.Key))
                    .ToList();

                foreach (var newTranslation in newTranslations)
                {
                    assetType.Translations.Add(new AssetTypeTranslation
                    {
                        LanguageCode = newTranslation.Key,
                        Name = newTranslation.Value
                    });
                }
            }

            await db.SaveChangesAsync();

            var response = new AssetTypeResponse
            {
                Id = assetType.Id,
                DefaultName = assetType.DefaultName,
                IsAsset = assetType.IsAsset,
                IsPhysical = assetType.IsPhysical,
                CreatedAt = assetType.CreatedAt,
                UpdatedAt = assetType.UpdatedAt,
                Translations = assetType.Translations.Select(t => new AssetTypeTranslationResponse
                {
                    Id = t.Id,
                    Name = t.Name,
                    LanguageCode = t.LanguageCode
                }).ToList()
            };

            return Results.Ok(response);
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        // Get a specific asset type by ID
        app.MapGet("/api/asset-types/{id}", async (int id, ApplicationDbContext db, string? lang) =>
        {
            var assetType = await db.AssetTypes
                .Include(at => at.Translations)
                .FirstOrDefaultAsync(at => at.Id == id);

            if (assetType == null)
                return Results.NotFound();

            var response = new AssetTypeResponse
            {
                Id = assetType.Id,
                DefaultName = assetType.DefaultName,
                IsAsset = assetType.IsAsset,
                IsPhysical = assetType.IsPhysical,
                CreatedAt = assetType.CreatedAt,
                UpdatedAt = assetType.UpdatedAt,
                Translations = assetType.Translations.Select(t => new AssetTypeTranslationResponse
                {
                    Id = t.Id,
                    Name = t.Name,
                    LanguageCode = t.LanguageCode
                }).ToList()
            };

            return Results.Ok(response);
        });
    }
}