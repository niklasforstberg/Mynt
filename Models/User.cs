using System;
using System.Collections.Generic;
using Mynt.Models.Enums;

namespace Mynt.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public string? Name { get; set; }
        public UserRole? Role { get; set; }
        public bool? Invited { get; set; } = false;
        public int? InvitedById { get; set; }
        public User? InvitedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<FinancialGroupMember> FinancialGroupMemberships { get; set; } = [];
        public ICollection<Asset> Assets { get; set; } = [];
        public ICollection<FinancialGroupInvitation> SentInvitations { get; set; } = [];
        public ICollection<UserActivity> UserActivities { get; set; } = [];
    }
} 