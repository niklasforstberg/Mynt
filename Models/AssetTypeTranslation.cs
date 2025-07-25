using System.Text.Json.Serialization;

namespace Mynt.Models
{
    public class AssetTypeTranslation
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string LanguageCode { get; set; }
        public int AssetTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        [JsonIgnore]
        public AssetType AssetType { get; set; } = null!;
    }
} 