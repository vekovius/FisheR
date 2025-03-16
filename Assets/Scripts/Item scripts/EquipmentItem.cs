using UnityEngine;

/// <summary>
/// Base class for all equipment items in the game.
/// Serves as foundation for equipment like fishing gear and apparel.
/// </summary>
public class EquipmentItem : ScriptableObject
{
    /// <summary>
    /// The display name of the item.
    /// </summary>
    public string itemName;

    /// <summary>
    /// A detailed description of the item's properties and effects
    /// </summary>
    public string description;

    /// <summary>
    /// The visual represntation of the item in inventory.
    /// </summary>
    public Sprite icon;

    /// <summary>
    /// The level of the item, which affercts its base stats and requirements
    /// </summary>
    public int itemLevel;

    /// <summary>
    /// Affects that player's casting power and fish fighitng ability
    /// </summary>
    public int strength;

    /// <summary>
    /// Affects the player's movement speed and agility when reeling in fish
    /// </summary>
    public int agility;

    /// <summary>
    /// Affects the player's ability to locate and attract fish, as well as their overall fishing skill
    /// </summary>
    public int intelligence;
}

/// <summary>
/// Represents the rarity of an equipment item,
/// which affects their stat ranges.
/// </summary>
public enum Rarity
{
    /// <summary>Basic items</summary>
    Common,

    /// <summary>Slighlty improved items</summary>
    Uncommon,

    /// <summary>Good items</summary>
    Rare,

    /// <summary>High-quality items</summary>
    Epic,

    /// <summary>Exceptional items with bis potentional</summary>
    Legendary
}
