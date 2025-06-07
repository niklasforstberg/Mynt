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
        app.MapGet("/api/asset-types", async (ApplicationDbContext db, string? lang) =>
        {
            var query = await db.AssetTypes
                .Include(at => at.Translations)
                .ToListAsync();

            return query.Select(at => new
            {
                at.Id,
                Name = !string.IsNullOrEmpty(lang) 
                    ? at.Translations.FirstOrDefault(t => t.LanguageCode == lang)?.Name ?? at.DefaultName
                    : at.DefaultName,
                at.IsAsset,
                at.IsPhysical
            });
        });

        // Create a new asset type with translations
        app.MapPost("/api/asset-types", async (AssetTypeCreateRequest request, ApplicationDbContext db) =>
        {
            var assetType = new AssetType
            {
                DefaultName = request.DefaultName,
                IsAsset = request.IsAsset,
                IsPhysical = request.IsPhysical
            };

            // Add translations
            foreach (var translation in request.Translations)
            {
                assetType.Translations.Add(new AssetTypeTranslation
                {
                    LanguageCode = translation.Key,
                    Name = translation.Value,
                    AssetType = assetType,
                    AssetTypeId = assetType.Id
                });
            }

            db.AssetTypes.Add(assetType);
            await db.SaveChangesAsync();

            return Results.Created($"/api/asset-types/{assetType.Id}", assetType);
        })
        .RequireAuthorization(policy => policy.RequireRole("Coach", "Admin"));
    }
} 