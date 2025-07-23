using System.ComponentModel.DataAnnotations;

namespace Mynt.Models
{
    public class Currency
    {
        /// <summary>
        /// ISO 4217 currency code (e.g., USD, EUR, SEK) - Primary Key
        /// </summary>
        [Required]
        [StringLength(3)]
        public required string Code { get; set; }

        /// <summary>
        /// Full currency name (e.g., US Dollar, Euro, Swedish Krona)
        /// </summary>
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        /// <summary>
        /// Currency symbol (e.g., $, €, kr)
        /// </summary>
        [StringLength(10)]
        public string? Symbol { get; set; }

        /// <summary>
        /// Whether this is a system-managed currency (true) or user-defined (false)
        /// </summary>
        public bool IsSystemManaged { get; set; } = true;

        /// <summary>
        /// User who created this currency (null for system currencies)
        /// </summary>
        public int? CreatedById { get; set; }

        /// <summary>
        /// When the currency was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Whether the currency is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public User? CreatedBy { get; set; }
        public ICollection<CurrencyExchangeRate> ExchangeRates { get; set; } = [];
        public ICollection<Asset> Assets { get; set; } = [];
    }
}