namespace Mynt.Models.DTOs.User;

public class UserInviteDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}