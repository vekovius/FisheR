using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment Template", menuName = "Fishing/Templates/Equipment Template")]
public class EquipmentTemplateAsset : ScriptableObject
{
    public string templateName;
    public Sprite icon;
    public AttributeFocus attributeFocus;
    public int baseStrength;
    public int baseAgility;
    public int baseIntelligence;

    public EquipmentTemplate GetTemplate()
    {
        EquipmentTemplate template = new EquipmentTemplate();
        template.name = templateName;
        template.icon = icon;
        template.attributeFocus = attributeFocus;
        template.baseStrength = baseStrength;
        template.baseAgility = baseAgility;
        template.baseIntelligence = baseIntelligence;
        return template;
    }
}