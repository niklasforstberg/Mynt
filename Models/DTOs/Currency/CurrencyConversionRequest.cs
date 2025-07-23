using System.ComponentModel.DataAnnotations;

namespace Mynt.Models.DTOs.Currency
{
    /// <summary>
    /// DTO for currency conversion requests
    /// </summary>
    public class CurrencyConversionRequest
    {
        /// <summary>
        /// Amount to convert
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue)]
        public required decimal Amount { get; set; }

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
    }
}