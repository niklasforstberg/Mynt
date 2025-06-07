namespace Mynt.Models
{
    public class AssetType
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public bool IsAsset { get; set; } = true;
        public bool IsPhysical { get; set; } = false;
    }
} 