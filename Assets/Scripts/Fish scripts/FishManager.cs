using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    [System.Serializable]
    public class  PopulationData
    {
        public FishType fishType; //Type of fish
        public int currentPopulation; //Current population of the fish type
        public float lastSpawnTime = 0f; //Last time a fish of this type was spawned
        public List<FishAI> activeFish = new List<FishAI>(); //List of active fish of this type
    }

    [Header("Ecosystem Settings")]
    public List<FishType> managedFishTypes = new List<FishType>(); //List of fish types to manage
    public List<SpawnRegion> spawnRegions = new List<SpawnRegion>(); //List of population data for each fish type
    public float populationUpdateInterval = 60f; //Interval for updating the population data in seconds
    public float timeScale = 1f; //Scale for time, used to speed up or slow down the simulation

    [Header("Debug")]
    public bool showPopulationStats = true;

    [Header("Population Data")]
    public Dictionary<string, PopulationData> populations = new Dictionary<string, PopulationData>(); //Dictionary to store population data for each fish type
    public float lastPopulationUpdateTime = 0f; //Last time the population data was updated
    public float gameTime = 0f; //Current game time in seconds


    private void Start()
    {
        
    }


}
