namespace Mynt.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public int? FinancialGroupId { get; set; }
        public required int UserId { get; set; }
        public int? AssetTypeId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public required DateTime CreatedAt { get; set; }

        public FinancialGroup? FinancialGroup { get; set; }
        public User? User { get; set; }
        public AssetType? AssetType { get; set; }
        public ICollection<AssetValue> AssetValues { get; set; } = [];

    }
} 