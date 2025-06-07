using System;

namespace Mynt.Models
{
    public class FinancialGroupInvitation
    {
        public int Id { get; set; }
        public int FinancialGroupId { get; set; }
        public string? InvitedEmail { get; set; }
        public required int InvitedByUserId { get; set; }
        public required string Status { get; set; }
        public Guid Token { get; set; }
        public required DateTime CreatedAt { get; set; }

        public required FinancialGroup FinancialGroup { get; set; }
        public User? InvitedByUser { get; set; }
    }
} 