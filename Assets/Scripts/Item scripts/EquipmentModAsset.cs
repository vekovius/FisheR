using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Equipment Mod", menuName = "Fishing/Mods/Equipment Mod")]
public class EquipmentModAsset : ScriptableObject
{
    public string modName;
    public List<ModTierAsset> tierAssets;

    public EquipmentMod GetEquipmentMod()
    {
        EquipmentMod mod = new EquipmentMod();
        mod.modName = modName;
        mod.tiers = new List<ModTier>();

        foreach (ModTierAsset tierAsset in tierAssets)
        {
            mod.tiers.Add(tierAsset.GetModTier());
        }

        return mod;
    }
}