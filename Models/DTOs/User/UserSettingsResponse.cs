namespace Mynt.Models.DTOs.User
{
    /// <summary>
    /// DTO for user settings response
    /// </summary>
    public class UserSettingsResponse
    {
        /// <summary>
        /// The user's preferred currency code (e.g., "USD", "EUR", "SEK")
        /// </summary>
        public string? PreferredCurrency { get; set; }
    }
}