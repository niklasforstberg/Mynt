using System.ComponentModel.DataAnnotations;

namespace Mynt.Models
{
    public class CurrencyExchangeRate
    {
        public int Id { get; set; }

        /// <summary>
        /// Source currency code
        /// </summary>
        [Required]
        [StringLength(3)]
        public required string FromCurrencyCode { get; set; }

        /// <summary>
        /// Target currency code
        /// </summary>
        [Required]
        [StringLength(3)]
        public required string ToCurrencyCode { get; set; }

        /// <summary>
        /// Exchange rate (1 FromCurrency = Rate ToCurrency)
        /// </summary>
        [Required]
        [Range(0.000001, double.MaxValue)]
        public required decimal Rate { get; set; }

        /// <summary>
        /// When this rate was effective from
        /// </summary>
        public required DateTime EffectiveFrom { get; set; }

        /// <summary>
        /// When this rate expires (null for current rate)
        /// </summary>
        public DateTime? EffectiveTo { get; set; }

        /// <summary>
        /// Source of the rate (API, Manual, etc.)
        /// </summary>
        [StringLength(50)]
        public string? Source { get; set; }

        /// <summary>
        /// User who manually set this rate (null for system rates)
        /// </summary>
        public int? SetById { get; set; }

        /// <summary>
        /// When the rate was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Currency FromCurrency { get; set; } = null!;
        public Currency ToCurrency { get; set; } = null!;
        public User? SetBy { get; set; }
    }
}