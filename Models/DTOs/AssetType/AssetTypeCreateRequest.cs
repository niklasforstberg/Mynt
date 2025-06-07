namespace Mynt.Models.DTOs.AssetType;

public class AssetTypeCreateRequest
{
    public required string Name { get; set; }
    public bool IsAsset { get; set; } = true;
    public bool IsPhysical { get; set; } = false;
} 