namespace Mynt.Models.DTOs.Asset;

public class AssetCreateRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? FinancialGroupId { get; set; }
    public int? AssetTypeId { get; set; }
} 