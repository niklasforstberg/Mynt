using Mynt.Models.Enums;

namespace Mynt.Models.DTOs.UserActivity;

public record UserActivityRequest
{
    public required int UserId { get; init; }
    public required UserActivityAction Action { get; init; }
    public string? Details { get; init; }
    public string? EntityType { get; init; }
    public string? EntityId { get; init; }
    public string? Status { get; init; } = "Success";
} 