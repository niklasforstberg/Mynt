using System;

namespace Mynt.Models
{
    public class FinancialGroupMember
    {
        public required int UserId { get; set; }
        public required int FinancialGroupId { get; set; }
        public required string Role { get; set; }

        public required User User { get; set; }
        public required FinancialGroup FinancialGroup { get; set; }
    }
} 