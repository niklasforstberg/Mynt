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
                    FinancialGroupName = fgm.FinancialGroup.Name ?? "Unknown",
                    Role = fgm.Role?.ToString() ?? "User",
                }).ToList()
            };

            return Results.Ok(response);
        })
        .RequireAuthorization(policy => policy.RequireRole("Coach", "Admin"));
    }
} 