using System;

namespace mynt.Models
{
    public class FinancialGroupInvitation
    {
        public int Id { get; set; }
        public int FinancialGroupId { get; set; }
        public string InvitedEmail { get; set; }
        public int InvitedByUserId { get; set; }
        public string Status { get; set; }
        public Guid Token { get; set; }
        public DateTime CreatedAt { get; set; }

        public FinancialGroup FinancialGroup { get; set; }
        public User InvitedByUser { get; set; }
    }
} 