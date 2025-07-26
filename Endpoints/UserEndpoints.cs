using Microsoft.EntityFrameworkCore;
using Mynt.Data;
using Mynt.Models.DTOs.User;
using Mynt.Models.Enums;

namespace Mynt.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/by-email/{email}", async (string email, ApplicationDbContext db) =>
        {
            var user = await db.Users
                .Include(u => u.InvitedBy)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return Results.NotFound();

            var response = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role?.ToString() ?? "User",
                Invited = user.Invited,
                InvitedBy = user.InvitedBy == null ? null : new UserInviterDto
                {
                    Id = user.InvitedBy.Id,
                    Email = user.InvitedBy.Email,
                },
                FinancialGroupMemberships = user.FinancialGroupMemberships.Select(fgm => new UserFinancialGroupDto
                {
                    FinancialGroupId = fgm.FinancialGroup.Id,
                    FinancialGroupName = fgm.FinancialGroup.Name ?? "Unknown"
                }).ToList(),
                Settings = new UserSettingsResponse
                {
                    PreferredCurrency = user.GetPreferredCurrency()
                }
            };

            return Results.Ok(response);
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        app.MapGet("/api/users/me", async (ApplicationDbContext db, HttpContext context) =>
        {
            var userEmail = context.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Results.Unauthorized();

            var user = await db.Users
                .Include(u => u.InvitedBy)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Results.NotFound();

            var response = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role?.ToString() ?? "User",
                Invited = user.Invited,
                InvitedBy = user.InvitedBy == null ? null : new UserInviterDto
                {
                    Id = user.InvitedBy.Id,
                    Email = user.InvitedBy.Email,
                },
                FinancialGroupMemberships = user.FinancialGroupMemberships.Select(fgm => new UserFinancialGroupDto
                {
                    FinancialGroupId = fgm.FinancialGroup.Id,
                    FinancialGroupName = fgm.FinancialGroup.Name ?? "Unknown"
                }).ToList(),
                Settings = new UserSettingsResponse
                {
                    PreferredCurrency = user.GetPreferredCurrency()
                }
            };

            return Results.Ok(response);
        })
        .RequireAuthorization();

        app.MapPut("/api/users/settings", async (UserSettingsRequest request, ApplicationDbContext db, HttpContext context) =>
        {
            var userEmail = context.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Results.Unauthorized();

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
                return Results.NotFound();

            user.SetPreferredCurrency(request.PreferredCurrency);
            await db.SaveChangesAsync();

            var response = new UserSettingsResponse
            {
                PreferredCurrency = user.GetPreferredCurrency()
            };

            return Results.Ok(response);
        })
        .RequireAuthorization();

        app.MapGet("/api/users/settings", async (ApplicationDbContext db, HttpContext context) =>
        {
            var userEmail = context.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Results.Unauthorized();

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
                return Results.NotFound();

            var response = new UserSettingsResponse
            {
                PreferredCurrency = user.GetPreferredCurrency()
            };

            return Results.Ok(response);
        })
        .RequireAuthorization();
    }
}