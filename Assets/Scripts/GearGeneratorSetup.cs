using UnityEngine;
using System.Collections.Generic;

public class GearGeneratorSetup : MonoBehaviour
{
    [Header("Rod Templates")]
    public List<EquipmentTemplateAsset> rodTemplateAssets;

    [Header("Reel Templates")]
    public List<EquipmentTemplateAsset> reelTemplateAssets;

    [Header("Line Templates")]
    public List<EquipmentTemplateAsset> lineTemplateAssets;

    [Header("Lure Templates")]
    public List<EquipmentTemplateAsset> lureTemplateAssets;

    [Header("Hat Templates")]
    public List<EquipmentTemplateAsset> hatTemplateAssets;

    [Header("Shirt Templates")]
    public List<EquipmentTemplateAsset> shirtTemplateAssets;

    [Header("Pants Templates")]
    public List<EquipmentTemplateAsset> pantsTemplateAssets;

    [Header("Boots Templates")]
    public List<EquipmentTemplateAsset> bootsTemplateAssets;

    [Header("Rod Mods")]
    public List<EquipmentModAsset> rodModAssets;

    [Header("Reel Mods")]
    public List<EquipmentModAsset> reelModAssets;

    [Header("Line Mods")]
    public List<EquipmentModAsset> lineModAssets;

    [Header("Lure Mods")]
    public List<EquipmentModAsset> lureModAssets;

    [Header("Hat Mods")]
    public List<EquipmentModAsset> hatModAssets;

    [Header("Shirt Mods")]
    public List<EquipmentModAsset> shirtModAssets;

    [Header("Pants Mods")]
    public List<EquipmentModAsset> pantsModAssets;

    [Header("Boots Mods")]
    public List<EquipmentModAsset> bootsModAssets;

    [Header("Attribute Mods")]
    public EquipmentModAsset strengthModAsset;
    public EquipmentModAsset agilityModAsset;
    public EquipmentModAsset intelligenceModAsset;

    public void Start()
    {
        GearGenerator gearGenerator = GetComponent<GearGenerator>();
        if (gearGenerator == null) return;

        // Set up templates
        SetupTemplates(rodTemplateAssets, ref gearGenerator.rodTemplates);
        SetupTemplates(reelTemplateAssets, ref gearGenerator.reelTemplates);
        SetupTemplates(lineTemplateAssets, ref gearGenerator.lineTemplates);
        SetupTemplates(lureTemplateAssets, ref gearGenerator.lureTemplates);
        SetupTemplates(hatTemplateAssets, ref gearGenerator.hatTemplates);
        SetupTemplates(shirtTemplateAssets, ref gearGenerator.shirtTemplates);
        SetupTemplates(pantsTemplateAssets, ref gearGenerator.pantsTemplates);
        SetupTemplates(bootsTemplateAssets, ref gearGenerator.bootsTemplates);

        // Set up mods
        SetupMods(rodModAssets, ref gearGenerator.rodMods);
        SetupMods(reelModAssets, ref gearGenerator.reelMods);
        SetupMods(lineModAssets, ref gearGenerator.lineMods);
        SetupMods(lureModAssets, ref gearGenerator.lureMods);
        SetupMods(hatModAssets, ref gearGenerator.hatMods);
        SetupMods(shirtModAssets, ref gearGenerator.shirtMods);
        SetupMods(pantsModAssets, ref gearGenerator.pantsMods);
        SetupMods(bootsModAssets, ref gearGenerator.bootsMods);

        // Set up attribute mods
        if (strengthModAsset != null)
            gearGenerator.strengthMod = strengthModAsset.GetEquipmentMod();

        if (agilityModAsset != null)
            gearGenerator.agilityMod = agilityModAsset.GetEquipmentMod();

        if (intelligenceModAsset != null)
            gearGenerator.intelligenceMod = intelligenceModAsset.GetEquipmentMod();
    }

    private void SetupTemplates(List<EquipmentTemplateAsset> assets, ref List<EquipmentTemplate> templates)
    {
        templates = new List<EquipmentTemplate>();
        foreach (EquipmentTemplateAsset asset in assets)
        {
            if (asset != null)
                templates.Add(asset.GetTemplate());
        }
    }

    private void SetupMods(List<EquipmentModAsset> assets, ref List<EquipmentMod> mods)
    {
        mods = new List<EquipmentMod>();
        foreach (EquipmentModAsset asset in assets)
        {
            if (asset != null)
                mods.Add(asset.GetEquipmentMod());
        }
    }
}