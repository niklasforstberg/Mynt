using System;
using System.Collections.Generic;

namespace Mynt.Models
{
    public class FinancialGroup
    {
        public int Id { get; set; }
        public required int OwnerId { get; set; }
        public string? Name { get; set; }
        public required DateTime CreatedAt { get; set; }

        public User? Owner { get; set; }
        public ICollection<FinancialGroupMember> Members { get; set; } = [];
        public ICollection<Asset> Assets { get; set; } = [];
        public ICollection<FinancialGroupInvitation> Invitations { get; set; } = [];
    }
} 