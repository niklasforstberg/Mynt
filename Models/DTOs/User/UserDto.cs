namespace mynt.Models.DTOs.User;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool? Invited { get; set; }
    public UserInviterDto? InvitedBy { get; set; }
    public List<UserFinancialGroupDto> FinancialGroupMemberships { get; set; } = new List<UserFinancialGroupDto>(); 
}