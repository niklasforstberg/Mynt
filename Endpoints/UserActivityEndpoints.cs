using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Mynt.Data;
using Mynt.Models;
using Mynt.Models.DTOs.UserActivity;
using Mynt.Models.Enums;

namespace Mynt.Endpoints;

public static class UserActivityEndpoints
{
    public static void MapUserActivityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/useractivity").WithTags("User Activity");

        // Log new activity
        group.MapPost("/", async (
            HttpContext context,
            ApplicationDbContext db,
            UserActivityRequest request) =>
        {
            var user = await db.Users.FindAsync(request.UserId) 
                ?? throw new InvalidOperationException("User not found");
                
            var userActivity = new UserActivity
            {
                UserId = request.UserId,
                User = user,
                Action = request.Action,
                Details = request.Details,
                Timestamp = DateTime.UtcNow,
                IPAddress = context.Connection.RemoteIpAddress?.ToString(),
                UserAgent = context.Request.Headers.UserAgent,
                EntityType = request.EntityType,
                EntityId = request.EntityId,
                Status = request.Status
            };
            db.UserActivities.Add(userActivity);
            await db.SaveChangesAsync();
            return Results.Ok(userActivity);
        })
        .RequireAuthorization();

        // Get activities for a specific user
        group.MapGet("/{userId}", async (
            int userId,
            ApplicationDbContext db,
            int? limit = 50) =>
        {
            var activities = await db.UserActivities
                .Where(ua => ua.UserId == userId)
                .OrderByDescending(ua => ua.Timestamp)
                .Take(limit ?? 50)
                .ToListAsync();

            return Results.Ok(activities);
        })
        .RequireAuthorization();
    }
}

// DTO for the request
public record UserActivityRequest
{
    public required int UserId { get; init; }
    public required UserActivityAction Action { get; init; }
    public string? Details { get; init; }
    public string? EntityType { get; init; }
    public string? EntityId { get; init; }
    public string? Status { get; init; } = "Success";
} 