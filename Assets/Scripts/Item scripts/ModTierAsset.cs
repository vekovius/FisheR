using UnityEngine;

[CreateAssetMenu(fileName = "New Mod Tier", menuName = "Fishing/Mods/Mod Tier")]
public class ModTierAsset : ScriptableObject
{
    public int tier;
    public int minItemLevel;
    public float minValue;
    public float maxValue;
    public float weight = 1f;

    public ModTier GetModTier()
    {
        ModTier modTier = new ModTier();
        modTier.tier = tier;
        modTier.minItemLevel = minItemLevel;
        modTier.minValue = minValue;
        modTier.maxValue = maxValue;
        modTier.weight = weight;
        return modTier;
    }
}