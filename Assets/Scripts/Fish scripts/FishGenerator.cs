using UnityEngine;

public class FishGenerator : MonoBehaviour
{
    //Rarity wegihts
    [Header("Rarity Generation Settings")]
    [SerializeField] public float commonWeight = 100f;
    [SerializeField] public float uncommonWeight = 50f;
    [SerializeField] public float rareWeight = 25f;
    [SerializeField] public float epicWeight = 5f;
    [SerializeField] public float legendaryWeight = 1f;

    //Fish modifier settings based on rarirty
    [Header("Fish Modifier Settings")]
    [SerializeField] private SerializableFishItem commonFishTemplate;
    [SerializeField] private SerializableFishItem uncommonFishTemplate;
    [SerializeField] private SerializableFishItem rareFishTemplate;
    [SerializeField] private SerializableFishItem epicFishTemplate;
    [SerializeField] private SerializableFishItem legendaryFishTemplate;

    

    //Generate fish with random rarity
    public SerializableFishItem GenerateRandomFish(FishType baseFishType)
    {
        Rarity rarity = GenerateRarity();
        return GenerateFish(baseFishType, rarity);
    }

    //Generate fish with specific rarity
    public SerializableFishItem GenerateFish(FishType baseFishType, Rarity rarity)
    {
        SerializableFishItem fish = new SerializableFishItem();
        fish.baseFishType = baseFishType;
        fish.fishName = baseFishType.speciesID;
        fish.rarity = rarity;

        //Apply rarity template
        ApplyRarityTemplate(fish, rarity);

        //Apply name
        fish.fishName = $"{rarity} {fish.fishName}";

        //Apply description
        UpdateFishDescription(fish);

        return fish;
    }
  
    //Helper method to generate rarity
    private Rarity GenerateRarity()
    {
        //Set weights
        float[] weights = new float[5] { commonWeight, uncommonWeight, rareWeight, epicWeight, legendaryWeight };

        //Calculate total weight
        float totalWeight = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            totalWeight += weights[i];
        }

        //Select rarity on weights
        float random = Random.Range(0, totalWeight);
        float accumulatedWeight = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            accumulatedWeight += weights[i];
            if (random < accumulatedWeight)
            {
                return (Rarity)i;
            }
        }

        return Rarity.Common;
    }

    //Apply template based on rarity
    private void ApplyRarityTemplate(SerializableFishItem fish, Rarity rarity)
    {
        SerializableFishItem template = null;

        switch (rarity)
        {
            case Rarity.Common:
                template = commonFishTemplate;
                break;
            case Rarity.Uncommon:
                template = uncommonFishTemplate;
                break;
            case Rarity.Rare:
                template = rareFishTemplate;
                break;
            case Rarity.Epic:
                template = epicFishTemplate;
                break;
            case Rarity.Legendary:
                template = legendaryFishTemplate;
                break;
        }

        if (template != null)
        {
            fish.speedMultiplier = template.speedMultiplier;
            fish.sizeMultiplier = template.sizeMultiplier;
            fish.forceMultiplier = template.forceMultiplier;
            fish.expValue = template.expValue;
            fish.gearDropChance = template.gearDropChance;
            fish.gearRarityBonus = template.gearRarityBonus;
            fish.glowEffect = template.glowEffect;
            fish.explicitModCount = template.explicitModCount;
        }
    }

    //Update fish description based on modifiers
    private void UpdateFishDescription(SerializableFishItem fish)
    {
        string description = $"{fish.rarity} {fish.baseFishType.speciesID}\n\n";

        if (fish.speedMultiplier != 1f)
        {
            description += $"Speed: {fish.speedMultiplier}x\n";
        }
        if (fish.sizeMultiplier != 1f)
        {
            description += $"Size: {fish.sizeMultiplier}x\n";
        }
        if (fish.forceMultiplier != 1f)
        {
            description += $"Force: {fish.forceMultiplier}x\n";
        }
        description += $"Exp Value: {fish.expValue}\n";
        description += $"Gear Drop Chance: {fish.gearDropChance}\n";

        if (fish.gearRarityBonus != 0)
        {
            description += $"Gear Rarity Bonus: {fish.gearRarityBonus}\n";
        }

        fish.description = description;
    }


}
