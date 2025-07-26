using System.Text.Json.Serialization;

namespace Mynt.Models.DTOs.Currency
{
    /// <summary>
    /// Response from Exchange Rates API
    /// </summary>
    public class ExchangeRatesApiResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("base")]
        public string Base { get; set; } = string.Empty;

        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;

        [JsonPropertyName("rates")]
        public Dictionary<string, decimal> Rates { get; set; } = new();
    }
}