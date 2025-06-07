namespace Mynt.Models.DTOs.Asset;

public class AssetResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public int? FinancialGroupId { get; set; }
    public string? FinancialGroupName { get; set; }
    public int? AssetTypeId { get; set; }
    public string? AssetTypeName { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal? CurrentValue { get; set; }
} 