using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates gear loot when fish are caught and adds them to player inventory
/// </summary>
public class FishLootGenerator : MonoBehaviour
{
    [Header("Loot Settings")]
    [Tooltip("Chance to generate gear when catching a fish (0-1)")]
    [Range(0f, 1f)]
    public float gearDropChance = 0.7f;

    [Tooltip("Minimum number of gear items to generate")]
    public int minGearItems = 1;

    [Tooltip("Maximum number of gear items to generate")]
    public int maxGearItems = 3;

    [Tooltip("Multiply fish rarity bonus by this value")]
    public float rarityBonusMultiplier = 1.5f;

    

    [Header("References")]
    public GearGenerator gearGenerator;
    public InventoryManager inventoryManager;

    void Start()
    {
        // Find the GearGenerator and InventoryManager in the scene
        
        //gearGenerator = FindObjectsOfType<GearGenerator>;
        if (gearGenerator == null)
        {
            Debug.LogError("GearGenerator not found in scene!");
        }

        
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found in scene!");
        }

        // Subscribe to fish caught event
        StateController.OnFishCaught += GenerateLootForFish;
    }

    void OnDestroy()
    {
        // Unsubscribe from event when destroyed
        StateController.OnFishCaught -= GenerateLootForFish;
    }

    /// <summary>
    /// Generates loot based on caught fish properties
    /// </summary>
    /// <param name="fish">The caught fish GameObject</param>
    private void GenerateLootForFish(GameObject fish)
    {
        if (gearGenerator == null || inventoryManager == null || fish == null)
            return;

        // Get fish AI component to access fish data
        FishAI fishAI = fish.GetComponent<FishAI>();
        if (fishAI == null || fishAI.fishData == null)
            return;

        // Determine if gear should drop based on fish's drop chance and global setting
        float finalDropChance = gearDropChance * fishAI.fishData.gearDropChance;
        if (Random.value > finalDropChance)
            return;

        // Determine number of gear items to generate
        int itemCount = Random.Range(minGearItems, maxGearItems + 1);

        // Apply any rarity modifier from fish data
        int rarityBonus = Mathf.RoundToInt(fishAI.fishData.gearRarityBonus * rarityBonusMultiplier);

        // Get base rarity from fish rarity
        Rarity baseRarity = fishAI.fishData.rarity;

        // Apply additional rarity bonus if any
        int rarityLevel = (int)baseRarity + rarityBonus;

        // Clamp to valid rarity range
        rarityLevel = Mathf.Clamp(rarityLevel, 0, System.Enum.GetValues(typeof(Rarity)).Length - 1);
        Rarity finalRarity = (Rarity)rarityLevel;

        // Set base item level based on fish size and rarity
        int baseItemLevel = Mathf.RoundToInt(10f * fishAI.fishData.sizeMultiplier * ((int)finalRarity + 1));
        baseItemLevel = Mathf.Clamp(baseItemLevel, 1, 30);

        // Generate and add gear items
        for (int i = 0; i < itemCount; i++)
        {
            // Choose random equipment type
            EquipmentType type = (EquipmentType)Random.Range(0, System.Enum.GetValues(typeof(EquipmentType)).Length);

            // Add small random variation to item level
            int itemLevel = baseItemLevel + Random.Range(-2, 3);
            itemLevel = Mathf.Max(1, itemLevel);

            // Generate the equipment item
            SerializableEquipmentItem item = gearGenerator.GetSerializableEquipment(type, itemLevel, finalRarity);

            // Add to inventory
            inventoryManager.AddItem(item);

            // Log for debugging
            Debug.Log($"Generated {item.itemName} from fish: {fishAI.fishData.fishName}");
        }
    }
}