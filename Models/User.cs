using System;
using System.Collections.Generic;

namespace Mynt.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? Name { get; set; }
        public UserRole? Role { get; set; }
        public bool? Invited { get; set; } = false;
        public int? InvitedById { get; set; }
        public User? InvitedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<FinancialGroupMember> FinancialGroupMemberships { get; set; }
        public ICollection<Asset> Assets { get; set; }
        public ICollection<FinancialGroupInvitation> SentInvitations { get; set; }
        
    }
} 