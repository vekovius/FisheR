using UnityEngine;


[CreateAssetMenu(fileName = "New Rod", menuName = "Fishing/Equipment/Rod")]
public class FishingRod : EquipmentItem
{
    public float castDistanceMultiplier;
    public float reelSpeedMultiplier;
}

[CreateAssetMenu(fileName = "New Reel", menuName = "Fishing/Equipment/Reel")]
public class FishingReel: EquipmentItem
{
    public float reelSpeed;
    public float tensionResistance;
}

[CreateAssetMenu(fileName = "New Line", menuName = "Fishing/Equipment/Line")]
public class FishingLine : EquipmentItem
{
    public float tensionLimit;
    public float visisbilityModifier;
}

[CreateAssetMenu(fileName = "New Lure", menuName = "Fishing/Equipment/Lure")]
public class FishingLure : EquipmentItem
{
    public float attraction;
    public float visibility;
}