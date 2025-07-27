namespace Mynt.Models
{
    public class AssetType
    {
        public int Id { get; set; }
        public required string DefaultName { get; set; }  // Default name (usually English)
        public bool IsAsset { get; set; } = true;
        public bool IsPhysical { get; set; } = false;
        public bool IsLiquid { get; set; } = true;  // Whether this asset type is easily convertible to cash
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<AssetTypeTranslation> Translations { get; set; } = new List<AssetTypeTranslation>();
    }
}