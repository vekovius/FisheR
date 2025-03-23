using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    [System.Serializable]
    public class PopulationData
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
        //InitializePopulationTracking
        foreach (FishType fishType in managedFishTypes)
        {
            PopulationData data = new PopulationData
            {
                fishType = fishType,
                currentPopulation = 0,
                lastSpawnTime = 0f,
                activeFish = new List<FishAI>()
            };

            populations[fishType.speciesID] = data; //Add the population data to the dictionary
        }

        //Inital population setup
        PerformIntialSpawning();
    }





    private void PerformIntialSpawning()
    {
        foreach (FishType fishType in managedFishTypes)
        {
            int initialCount = fishType.targetPopulation / 2; //Spawn half

            //Find stuitable spawn region for the fish type
            List<SpawnRegion> suitableRegions = FindSuitableRegions(fishType);
            if (suitableRegions.Count > 0)
            {
                int fishPerRegion = initialCount / suitableRegions.Count; //Distribute fish evenly across suitable regions
                int remainder = initialCount % suitableRegions.Count;

                for (int i = 0; i < suitableRegions.Count; i++)
                {
                    int fishToSpawn = fishPerRegion;
                    if (i == 0) fishToSpawn += remainder; //Add any remainder to the first region

                    SpawnFishGroup(fishType, suitableRegions[i], fishToSpawn);
                }
            }
        }
    }

    private List<SpawnRegion> FindSuitableRegions(FishType fishType)
    {
        List<SpawnRegion> suitable = new List<SpawnRegion>();

        foreach (SpawnRegion region in spawnRegions)
        {
            if (region.speciesID == fishType.speciesID)
            {
                suitable.Add(region);
            }
        }
        return suitable;
    }

    private void SpawnFishGroup(FishType fishType, SpawnRegion region, int count)
    {
        //Create a parent object for the school
        GameObject schoolParent = new GameObject(fishType.speciesID + " School");

        //Get population data
        PopulationData populationData = populations[fishType.speciesID];

        //Random postion within region for school center
        Vector2 schoolCenter = region.GetRandomPosition();
        schoolParent.transform.position = schoolCenter;

        //Spawn indivudual fishies
        for (int i = 0; i < count; i++)
        {
            //Random position within the region for each fish
            Vector2 spawnPosition = schoolCenter + new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));

            //Create fish instance
            GameObject fishObj = Instantiate(fishType.prefab, spawnPosition, Quaternion.identity);
            fishObj.transform.parent = schoolParent.transform; //Set the parent to the school object

            //Set up fish AI
            FishAI fishAI = fishObj.GetComponent<FishAI>();
            if (fishAI == null)
            {
                fishAI = fishObj.AddComponent<FishAI>(); //Add FishAI component if not present
            }

            fishAI.Initialize(fishType, region, schoolCenter); //Initialize fish AI with type and region

            populationData.activeFish.Add(fishAI); //Add fish to the active list
            populationData.currentPopulation++; //Increment the current population

        }
        populationData.lastSpawnTime = gameTime; //Update the last spawn time for this fish type
    }




    private void OnGUI()
    {
        if(!showPopulationStats) return; //Exit if we don't want to show stats

        int y = 10;
        
        foreach (var entry in populations)
        {
            GUI.Label(new Rect(10, y, 300, 20), $"{entry.Value.fishType.speciesID}: {entry.Value.currentPopulation}/{entry.Value.fishType.targetPopulation} fish");
            y += 20; //Move down for the next label
        }
    }

}
