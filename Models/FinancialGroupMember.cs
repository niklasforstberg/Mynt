using System;

namespace mynt.Models
{
    public class FinancialGroupMember
    {
        public int UserId { get; set; }
        public int FinancialGroupId { get; set; }
        public string Role { get; set; }

        public User User { get; set; }
        public FinancialGroup FinancialGroup { get; set; }
    }
} 