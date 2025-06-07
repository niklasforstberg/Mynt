namespace Mynt.Models
{
    public class AssetType
    {
        public int Id { get; set; }
        public required string DefaultName { get; set; }  // Default name (usually English)
        public bool IsAsset { get; set; } = true;
        public bool IsPhysical { get; set; } = false;
        
        public ICollection<AssetTypeTranslation> Translations { get; set; } = new List<AssetTypeTranslation>();
    }
} 