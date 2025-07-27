using System;
using System.Collections.Generic;
using System.Text.Json;
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
        public string? Settings { get; set; }

        public ICollection<FinancialGroupMember> FinancialGroupMemberships { get; set; } = [];
        public ICollection<Asset> Assets { get; set; } = [];
        public SnapshotConfiguration? SnapshotConfiguration { get; set; }

        /// <summary>
        /// Gets the user's preferred currency code from settings
        /// </summary>
        public string? GetPreferredCurrency()
        {
            if (string.IsNullOrEmpty(Settings))
                return null;

            try
            {
                var settings = JsonSerializer.Deserialize<Dictionary<string, object>>(Settings);
                return settings?.GetValueOrDefault("preferredCurrency")?.ToString();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Sets the user's preferred currency code in settings
        /// </summary>
        public void SetPreferredCurrency(string currencyCode)
        {
            var settings = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(Settings))
            {
                try
                {
                    settings = JsonSerializer.Deserialize<Dictionary<string, object>>(Settings) ?? new();
                }
                catch
                {
                    settings = new();
                }
            }

            settings["preferredCurrency"] = currencyCode;
            Settings = JsonSerializer.Serialize(settings);
        }
    }
}