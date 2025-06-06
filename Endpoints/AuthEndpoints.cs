using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Mynt.Data;
using Mynt.Models;
using Mynt.Models.DTOs.Auth;
using Mynt.Models.DTOs.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Mynt.Models.Enums;

namespace Mynt.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        // Public endpoints (no auth required)
        app.MapPost("/api/auth/login", async (LoginDto request, ApplicationDbContext db, IConfiguration config, HttpContext context, IUserActivityService activityService) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Results.Unauthorized();
            }

            var token = GenerateJwtToken(user, config);

            // Log the login activity
            await activityService.LogActivityAsync(
                user.Id, 
                UserActivityAction.Login,
                "User logged in successfully"
            );
            await db.SaveChangesAsync();

            return Results.Ok(new { Token = token });
        });

        app.MapPost("/api/auth/register", async (RegisterDto request, ApplicationDbContext db, IConfiguration config, IUserActivityService activityService) =>
        {
            if (await db.Users.AnyAsync(u => u.Email == request.Email))
            {
                return Results.BadRequest("Email already registered");
            }

            var userRole = UserRole.User;

            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = userRole
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            var token = GenerateJwtToken(user, config);

            await activityService.LogActivityAsync(
                user.Id, 
                UserActivityAction.Register,
                "User registered successfully"
            );
            return Results.Ok(new { Token = token });
        });

        app.MapPost("/api/auth/setup/first-admin", async (CreateAdminDto request, ApplicationDbContext db, IConfiguration config, IUserActivityService activityService) =>
        {
            if (await db.Users.AnyAsync(u => u.Role == UserRole.Admin))
            {
                return Results.BadRequest("Admin already exists. Use regular admin creation endpoint.");
            }

            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = UserRole.Admin
            };

            await activityService.LogActivityAsync(
                user.Id, 
                UserActivityAction.SetupFirstAdmin,
                "First admin setup successfully"
            );

            db.Users.Add(user);
            await db.SaveChangesAsync();

            var token = GenerateJwtToken(user, config);
            return Results.Ok(new { Token = token });
        });

        // Protected endpoints (auth required)
        app.MapGet("/api/auth/me", [Authorize] async (HttpContext context, ApplicationDbContext db) =>
        {           
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Results.Unauthorized();

            var user = await db.Users
                .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

            if (user == null)
                return Results.NotFound();

            var response = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role.ToString(),
                Invited = user.Invited,
                InvitedBy = user.InvitedBy == null ? null : new UserInviterDto
                {
                    Id = user.InvitedBy.Id,
                    Email = user.InvitedBy.Email,
                }
            };

            return Results.Ok(response);
        });

        // Admin endpoints
        app.MapPost("/api/auth/admin/create", async (CreateAdminDto request, ApplicationDbContext db, IUserActivityService activityService) =>
        {
            if (await db.Users.AnyAsync(u => u.Email == request.Email))
            {
                return Results.BadRequest("Email already registered");
            }

            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = UserRole.Admin
            };

            await activityService.LogActivityAsync(
                user.Id, 
                UserActivityAction.CreateAdmin,
                "Admin user created successfully"
            );

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Results.Ok("Admin user created successfully");
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }

    private static string GenerateJwtToken(User user, IConfiguration config)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email ?? throw new InvalidOperationException("User email is required")),
            new Claim(ClaimTypes.Role, user.Role.ToString() ?? throw new InvalidOperationException("User role is required")),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? throw new InvalidOperationException("User ID is required"))
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
} 