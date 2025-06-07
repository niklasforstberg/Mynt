using Microsoft.EntityFrameworkCore;
using Mynt.Data;
using Mynt.Models;
using Mynt.Models.DTOs.AssetType;

namespace Mynt.Endpoints;

public static class AssetTypeEndpoints
{
    public static void MapAssetTypeEndpoints(this IEndpointRouteBuilder app)
    {
        // Get all asset types
        app.MapGet("/api/asset-types", async (ApplicationDbContext db) =>
        {
            var assetTypes = await db.AssetTypes.ToListAsync();
            return Results.Ok(assetTypes);
        });

        // Create a new asset type
        app.MapPost("/api/asset-types", async (AssetTypeCreateRequest request, ApplicationDbContext db) =>
        {
            var assetType = new AssetType
            {
                Name = request.Name,
                IsAsset = request.IsAsset,
                IsPhysical = request.IsPhysical
            };

            db.AssetTypes.Add(assetType);
            await db.SaveChangesAsync();

            return Results.Created($"/api/asset-types/{assetType.Id}", assetType);
        })
        .RequireAuthorization(policy => policy.RequireRole("Coach", "Admin"));
    }
}

public class AssetTypeCreateRequest
{
    public required string Name { get; set; }
    public bool IsAsset { get; set; } = true;
    public bool IsPhysical { get; set; } = false;
} 