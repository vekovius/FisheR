#if UNITY_EDITOR
using static UnityEngine.GraphicsBuffer;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GearGenerator))]
public class GearGeneratorEditor : Editor
{
    private EquipmentType testType = EquipmentType.Rod;
    private int testLevel = 10;
    private Rarity testRarity = Rarity.Rare;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GearGenerator generator = (GearGenerator)target;

        GUILayout.Space(10);
        GUILayout.Label("Test Equipment Generation", EditorStyles.boldLabel);

        testType = (EquipmentType)EditorGUILayout.EnumPopup("Equipment Type", testType);
        testLevel = EditorGUILayout.IntSlider("Item Level", testLevel, 1, 30);
        testRarity = (Rarity)EditorGUILayout.EnumPopup("Rarity", testRarity);

        if (GUILayout.Button("Generate Test Item"))
        {
            generator.DubugGenerateAndPrint(testType, testLevel, testRarity);
        }
    }
}
#endif