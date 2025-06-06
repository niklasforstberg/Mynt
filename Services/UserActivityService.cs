using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Mynt.Models;
using Mynt.Data;
using Mynt.Models.Enums;

public interface IUserActivityService
{
    Task LogActivityAsync(int userId, UserActivityAction action, string details, 
        string? entityType = null, string? entityId = null);
}

public class UserActivityService : IUserActivityService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserActivityService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogActivityAsync(int userId, UserActivityAction action, string details, 
        string? entityType = null, string? entityId = null)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) throw new InvalidOperationException("User not found");

        var context = _httpContextAccessor.HttpContext;
        
        var activity = new UserActivity
        {
            UserId = userId,
            User = user,
            Action = action,
            Details = details,
            EntityType = entityType,
            EntityId = entityId,
            Timestamp = DateTime.UtcNow,
            IPAddress = context?.Connection.RemoteIpAddress?.ToString(),
            UserAgent = context?.Request.Headers.UserAgent,
            Status = "Success"
        };

        await _db.UserActivities.AddAsync(activity);
        await _db.SaveChangesAsync();
    }
} 