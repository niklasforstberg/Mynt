using Microsoft.EntityFrameworkCore;
using Mynt.Data;
using Mynt.Models;
using Mynt.Models.DTOs.AssetValue;
using System.Security.Claims;

namespace Mynt.Endpoints;

public static class AssetValueEndpoints
{
    public static void MapAssetValueEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/asset-values").RequireAuthorization();

        // CREATE: Create a new asset value record
        group.MapPost("/", async (AssetValueCreateRequest request, ApplicationDbContext db, HttpContext context) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            // Verify the user owns the asset
            var asset = await db.Assets
                .FirstOrDefaultAsync(a => a.Id == request.AssetId && a.UserId == userId);
            
            if (asset == null)
                return Results.BadRequest("Asset not found or you don't have permission to add values to it.");

            var assetValue = new AssetValue
            {
                AssetId = request.AssetId,
                Value = request.Value,
                RecordedAt = request.RecordedAt,
                Asset = asset
            };

            db.AssetValues.Add(assetValue);
            await db.SaveChangesAsync();

            var response = new AssetValueResponse
            {
                Id = assetValue.Id,
                AssetId = assetValue.AssetId,
                AssetName = asset.Name ?? "",
                Value = assetValue.Value,
                RecordedAt = assetValue.RecordedAt
            };

            return Results.Created($"/api/asset-values/{assetValue.Id}", response);
        });

        // READ: Get all asset values for assets owned by the current user
        group.MapGet("/", async (ApplicationDbContext db, HttpContext context, int? assetId = null) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var query = db.AssetValues
                .Include(av => av.Asset)
                .Where(av => av.Asset.UserId == userId);

            // Filter by specific asset if provided
            if (assetId.HasValue)
            {
                query = query.Where(av => av.AssetId == assetId.Value);
            }

            var assetValues = await query
                .OrderByDescending(av => av.RecordedAt)
                .ToListAsync();

            var response = assetValues.Select(av => new AssetValueResponse
            {
                Id = av.Id,
                AssetId = av.AssetId,
                AssetName = av.Asset.Name ?? "",
                Value = av.Value,
                RecordedAt = av.RecordedAt
            });

            return Results.Ok(response);
        });

        // READ: Get a specific asset value by ID
        group.MapGet("/{id}", async (int id, ApplicationDbContext db, HttpContext context) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var assetValue = await db.AssetValues
                .Include(av => av.Asset)
                .FirstOrDefaultAsync(av => av.Id == id && av.Asset.UserId == userId);

            if (assetValue == null)
                return Results.NotFound();

            var response = new AssetValueResponse
            {
                Id = assetValue.Id,
                AssetId = assetValue.AssetId,
                AssetName = assetValue.Asset.Name ?? "",
                Value = assetValue.Value,
                RecordedAt = assetValue.RecordedAt
            };

            return Results.Ok(response);
        });

        // UPDATE: Update an existing asset value
        group.MapPut("/{id}", async (int id, AssetValueUpdateRequest request, ApplicationDbContext db, HttpContext context) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var assetValue = await db.AssetValues
                .Include(av => av.Asset)
                .FirstOrDefaultAsync(av => av.Id == id && av.Asset.UserId == userId);
            
            if (assetValue == null)
                return Results.NotFound();

            assetValue.Value = request.Value;
            assetValue.RecordedAt = request.RecordedAt;

            await db.SaveChangesAsync();

            var response = new AssetValueResponse
            {
                Id = assetValue.Id,
                AssetId = assetValue.AssetId,
                AssetName = assetValue.Asset.Name ?? "",
                Value = assetValue.Value,
                RecordedAt = assetValue.RecordedAt
            };

            return Results.Ok(response);
        });

        // DELETE: Delete an asset value
        group.MapDelete("/{id}", async (int id, ApplicationDbContext db, HttpContext context) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var assetValue = await db.AssetValues
                .Include(av => av.Asset)
                .FirstOrDefaultAsync(av => av.Id == id && av.Asset.UserId == userId);
            
            if (assetValue == null)
                return Results.NotFound();

            db.AssetValues.Remove(assetValue);
            await db.SaveChangesAsync();
            
            return Results.Ok();
        });

        // Additional endpoint: Get asset value history for a specific asset
        group.MapGet("/asset/{assetId}/history", async (int assetId, ApplicationDbContext db, HttpContext context) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            // Verify the user owns the asset
            var asset = await db.Assets
                .FirstOrDefaultAsync(a => a.Id == assetId && a.UserId == userId);
            
            if (asset == null)
                return Results.NotFound("Asset not found or you don't have permission to view its values.");

            var assetValues = await db.AssetValues
                .Where(av => av.AssetId == assetId)
                .OrderByDescending(av => av.RecordedAt)
                .ToListAsync();

            var response = assetValues.Select(av => new AssetValueResponse
            {
                Id = av.Id,
                AssetId = av.AssetId,
                AssetName = asset.Name ?? "",
                Value = av.Value,
                RecordedAt = av.RecordedAt
            });

            return Results.Ok(response);
        });
    }
} 