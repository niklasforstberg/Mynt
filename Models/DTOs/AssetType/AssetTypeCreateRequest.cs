namespace Mynt.Models.DTOs.AssetType;

public class AssetTypeCreateRequest
{
    public required string DefaultName { get; set; }
    public bool IsAsset { get; set; } = true;
    public bool IsPhysical { get; set; } = false;
    public Dictionary<string, string> Translations { get; set; } = new();
} 