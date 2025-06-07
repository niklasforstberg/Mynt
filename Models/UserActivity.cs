using System;
using Mynt.Models.Enums;

namespace Mynt.Models
{
    public class UserActivity
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public UserActivityAction Action { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }  // Browser/client information
        public string? EntityType { get; set; }  // The type of entity being acted upon (e.g., "Asset", "FinancialGroup")
        public string? EntityId { get; set; }    // The ID of the entity being acted upon
        public string? Status { get; set; }      // Success/Failure/Pending
        public string? ErrorMessage { get; set; } // For storing any error information

        // Navigation property
        public User? User { get; set; }
    }
} 