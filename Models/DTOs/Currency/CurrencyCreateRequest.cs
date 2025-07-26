using System.ComponentModel.DataAnnotations;

namespace Mynt.Models.DTOs.Currency
{
    /// <summary>
    /// DTO for creating new currencies
    /// </summary>
    public class CurrencyCreateRequest
    {
        /// <summary>
        /// ISO 4217 currency code (e.g., USD, EUR, SEK)
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
        /// Position of the symbol relative to the value
        /// </summary>
        public SymbolPosition SymbolPosition { get; set; } = SymbolPosition.Before;
    }
}