using System.ComponentModel.DataAnnotations;

namespace Mynt.Models.DTOs.Currency
{
    /// <summary>
    /// DTO for setting exchange rates
    /// </summary>
    public class CurrencyExchangeRateRequest
    {
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
        /// Source of the rate (API, Manual, etc.)
        /// </summary>
        [StringLength(50)]
        public string? Source { get; set; }
    }
}