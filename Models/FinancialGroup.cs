using System;
using System.Collections.Generic;

namespace mynt.Models
{
    public class FinancialGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<FinancialGroupMember> Members { get; set; }
        public ICollection<Asset> Assets { get; set; }
        public ICollection<FinancialGroupInvitation> Invitations { get; set; }
    }
} 