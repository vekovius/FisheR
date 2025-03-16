using UnityEngine;

[CreateAssetMenu(fileName = "New Hat", menuName = "Fishing/Apparel/Hat")]
public class Hat : EquipmentItem
{
    public float fishPerceptionRange;
}

[CreateAssetMenu(fileName = "New Shirt", menuName = "Fishing/Apparel/Shirt")]
public class Shirt : EquipmentItem
{
    public float staminaRegenRate;
    public int extraInventorySlots;
}

[CreateAssetMenu(fileName = "New Pants", menuName = "Fishing/Apparel/Pants")]
public class Pants : EquipmentItem
{
    public float fishStruggleResistance;
}

[CreateAssetMenu(fileName = "New Boots", menuName = "Fishing/Apparel/Boots")]
public class Boots : EquipmentItem
{
    public float moveSpeedBonus;
}
