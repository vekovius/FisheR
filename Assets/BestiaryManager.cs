using System.Collections.Generic;
using UnityEngine;

public class BestiaryManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public Dictionary<SerializableFishItem, bool> discoveredFish = new Dictionary<SerializableFishItem, bool>();

    // Call this when the player catches a new fish
    public void DiscoverFish(SerializableFishItem fish)
    {
        if (!discoveredFish.ContainsKey(fish))
        {
            discoveredFish.Add(fish, true);
            Debug.Log($"Discovered new fish: {fish.fishName}");
        }
    }

    // Check if a fish has been discovered
    public bool IsFishDiscovered(SerializableFishItem fish)
    {
        return discoveredFish.ContainsKey(fish);
    }
}

