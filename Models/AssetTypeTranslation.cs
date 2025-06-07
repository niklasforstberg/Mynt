namespace Mynt.Models
{
    public class AssetTypeTranslation
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string LanguageCode { get; set; }
        public int AssetTypeId { get; set; }
        public AssetType AssetType { get; set; } = null!;
    }
} 