using Microsoft.EntityFrameworkCore;
using Mynt.Data;
using Mynt.Models;
using Mynt.Models.DTOs.Asset;
using System.Security.Claims;

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
                CreatedAt = DateTime.UtcNow
            };

            db.Assets.Add(asset);
            await db.SaveChangesAsync();

            return Results.Created($"/api/assets/{asset.Id}", asset);
        });

        // READ: Get all assets for the current user
        group.MapGet("/", async (ApplicationDbContext db, HttpContext context) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

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
                AssetTypeName = a.AssetType?.Name,
                CreatedAt = a.CreatedAt,
                CurrentValue = a.AssetValues.FirstOrDefault()?.Value
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
                AssetTypeName = asset.AssetType?.Name,
                CreatedAt = asset.CreatedAt,
                CurrentValue = asset.AssetValues.FirstOrDefault()?.Value
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

            await db.SaveChangesAsync();
            return Results.Ok(asset);
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
    }
}

public class AssetCreateRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? FinancialGroupId { get; set; }
    public int? AssetTypeId { get; set; }
}

public class AssetUpdateRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? FinancialGroupId { get; set; }
    public int? AssetTypeId { get; set; }
}

public class AssetResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public int? FinancialGroupId { get; set; }
    public string? FinancialGroupName { get; set; }
    public int? AssetTypeId { get; set; }
    public string? AssetTypeName { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal? CurrentValue { get; set; }
} 