using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Mynt.Models.DTOs.AssetType;

/// <summary>
/// Request to create a new asset type with multilingual support
/// </summary>
public class AssetTypeCreateRequest
{
    /// <summary>
    /// The default name for the asset type (usually in English)
    /// </summary>
    /// <example>Cash</example>
    public required string DefaultName { get; set; }

    /// <summary>
    /// Whether this represents an asset (true) or liability (false)
    /// </summary>
    public bool IsAsset { get; set; } = true;

    /// <summary>
    /// Whether this is a physical asset
    /// </summary>
    public bool IsPhysical { get; set; } = false;

    /// <summary>
    /// Whether this asset type is easily convertible to cash
    /// </summary>
    public bool IsLiquid { get; set; } = true;

    /// <summary>
    /// Translations for different languages. Key is language code (en, es, fr, de), value is translated name.
    /// </summary>
    /// <example>
    /// {
    ///   "en": "Cash",
    ///   "es": "Efectivo",
    ///   "fr": "Espèces",
    ///   "de": "Bargeld"
    /// }
    /// </example>
    public Dictionary<string, string> Translations { get; set; } = new();
}