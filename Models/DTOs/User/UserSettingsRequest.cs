namespace Mynt.Models.DTOs.User
{
    /// <summary>
    /// DTO for updating user settings
    /// </summary>
    public class UserSettingsRequest
    {
        /// <summary>
        /// The user's preferred currency code (e.g., "USD", "EUR", "SEK")
        /// </summary>
        public required string PreferredCurrency { get; set; }
    }
}