using UnityEngine;

public class EquipmentItem : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public int itemLevel;
    public int strength;
    public int agility;
    public int intelligence;
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
