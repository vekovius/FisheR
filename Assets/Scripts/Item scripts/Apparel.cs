using UnityEngine;

/// <summary>
/// Represents a hat that can be equipped by the player.
/// Hats primariliy afferct the player's ability see fish.
/// </summary>
[CreateAssetMenu(fileName = "New Hat", menuName = "Fishing/Apparel/Hat")]
public class Hat : EquipmentItem
{
    /// <summary>
    /// Increases the range at which the palyer can detect fish in the water.
    /// </summary>
    public float fishPerceptionRange;
}

/// <summary>
/// Represents a shirt that can be equipped by the player.
/// Shirts affect the player's stamina and inventory capacity.
/// </summary>
[CreateAssetMenu(fileName = "New Shirt", menuName = "Fishing/Apparel/Shirt")]
public class Shirt : EquipmentItem
{
    /// <summary>
    /// Rate at which the palyer's stamina regenerates over time.
    /// </summary>
    public float staminaRegenRate;

    /// <summary>
    /// Unlocks additional inventory slots for the player, allowing them to carry more items.
    /// </summary>
    public int extraInventorySlots;
}

/// <summary>
/// Represents pants that can be equipped by the player.
/// </summary>
[CreateAssetMenu(fileName = "New Pants", menuName = "Fishing/Apparel/Pants")]
public class Pants : EquipmentItem
{
    /// <summary>
    /// Increases the player's resistance to fish struggle, making it easier to reel in fish.
    /// </summary>
    public float fishStruggleResistance;
}

/// <summary>
/// Represents boots that can be equipped by the player.
/// </summary>
[CreateAssetMenu(fileName = "New Boots", menuName = "Fishing/Apparel/Boots")]
public class Boots : EquipmentItem
{
    /// <summary>
    /// Increases the player's movement speed
    /// </summary>
    public float moveSpeedBonus;
}
