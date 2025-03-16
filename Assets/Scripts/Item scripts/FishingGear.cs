using UnityEngine;

/// <summary>
/// Represents a fishing rod that can be equipped by the player.
/// Fishing rods determine the casting distance, accuracy, and power rating.
/// </summary>
[CreateAssetMenu(fileName = "New Rod", menuName = "Fishing/Equipment/Rod")]
public class FishingRod : EquipmentItem
{
    /// <summary>
    /// Multiplier that increases the maximum distance a player cast cast
    /// </summary>
    public float CastDistanceMultiplier;

    /// <summary>
    /// Affects the casting accuracy and reduces casting deviation
    /// </summary>
    public float Accuracy;

    /// <summary>
    /// Dtermines how well the rod handles larger fish and reduces fish escape chance
    /// </summary>
    public float PowerRating;
}

/// <summary>
/// Represents a fishing reel that can be equipped by the player.
/// Fishing reels control the line retrieval speed, drag settings, and overall line management.
/// </summary>
[CreateAssetMenu(fileName = "New Reel", menuName = "Fishing/Equipment/Reel")]
public class FishingReel : EquipmentItem
{
    /// <summary>
    /// Base speed at which the player can reel in fish
    /// </summary>
    public float ReelSpeed;

    /// <summary>
    /// How well the rell can handle tension when fighting strong fish
    /// </summary>
    public float TensionResistance;
}

/// <summary>
/// Represents a fishing line that can be equipped by the player.
/// Fishing lines affect the maximum tension that can be applied and how long the line is.
/// </summary>
[CreateAssetMenu(fileName = "New Line", menuName = "Fishing/Equipment/Line")]
public class FishingLine : EquipmentItem
{
    /// <summary>
    /// Maximum tension that can be applied to the line before it breaks
    /// </summary>
    public float TensionLimit;

    /// <summary>
    /// Length of the fishing line, affects max line distance
    /// </summary>
    public float LineLength;
}

/// <summary>
/// Represents a fishing lure that can be equipped by the player.
/// </summary>
[CreateAssetMenu(fileName = "New Lure", menuName = "Fishing/Equipment/Lure")]
public class FishingLure : EquipmentItem
{
    /// <summary>
    /// How effective the lure is at attracting fish
    /// </summary>
    public float Attraction;

    /// <summary>
    /// How visible the lure is to fish, affects the chance of fish noticing it
    /// </summary>
    public float Visibility;
}
