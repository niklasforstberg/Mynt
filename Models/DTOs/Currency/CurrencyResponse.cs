namespace Mynt.Models.DTOs.Currency
{
    /// <summary>
    /// DTO for currency responses
    /// </summary>
    public class CurrencyResponse
    {
        /// <summary>
        /// ISO 4217 currency code (Primary Key)
        /// </summary>
        public required string Code { get; set; }

        /// <summary>
        /// Full currency name
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Currency symbol
        /// </summary>
        public string? Symbol { get; set; }

        /// <summary>
        /// Position of the symbol relative to the value
        /// </summary>
        public SymbolPosition SymbolPosition { get; set; }

        /// <summary>
        /// Whether this is a system-managed currency
        /// </summary>
        public bool IsSystemManaged { get; set; }

        /// <summary>
        /// Whether the currency is active
        /// </summary>
        public bool IsActive { get; set; }
    }
}