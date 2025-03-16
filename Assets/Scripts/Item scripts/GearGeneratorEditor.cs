using UnityEditor;
using UnityEngine;

/// <summary>
/// Cusom editor for the GearGenerator component.
/// </summary>
/// <remarks> 
/// This editor extends the default insprector for GearGenerator and adds a
/// testing section that allows the testing of equipment generator 
/// directly from the inspector without needing to run the game.
/// </remarks>
[CustomEditor(typeof(GearGenerator))]
public class GearGeneratorEditor : Editor
{
    private EquipmentType testType = EquipmentType.Rod;
    private int testLevel = 10;
    private Rarity testRarity = Rarity.Rare;

    /// <summary>
    /// Override the OnInspectorGUI method to customize the inspector for GearGenerator.
    /// </summary>
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GearGenerator generator = (GearGenerator)target;

        GUILayout.Space(10);
        GUILayout.Label("Test Equipment Generation", EditorStyles.boldLabel);

        testType = (EquipmentType)EditorGUILayout.EnumPopup("Equipment Type", testType);
        testLevel = EditorGUILayout.IntSlider("Item Level", testLevel, 1, 30);
        testRarity = (Rarity)EditorGUILayout.EnumPopup("Rarity", testRarity);

        /// Button to generate and print the test item
        if (GUILayout.Button("Generate Test Item"))
        {
            generator.DubugGenerateAndPrint(testType, testLevel, testRarity);
        }
    }
}
