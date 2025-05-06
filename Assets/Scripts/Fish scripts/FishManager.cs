using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishManager : MonoBehaviour
{
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
    
    [Header("Fish Quantity Settings")]
    [Tooltip("Multiplier for target population. Higher values = more fish")]
    [Range(1f, 10f)]
    public float populationMultiplier = 5f; // Significantly increased to 5x more fish

    [Header("Debug")]
    public bool showPopulationStats = true;

    [Header("Population Data")]
    public Dictionary<string, PopulationData> populations = new Dictionary<string, PopulationData>(); //Dictionary to store population data for each fish type
    public float lastPopulationUpdateTime = 0f; //Last time the population data was updated
    public float gameTime = 0f; //Current game time in seconds

    [Header("Fish Generator")]
    public FishGenerator fishGenerator;

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
            };

            populations[fishType.speciesID] = data; //Add the population data to the dictionary
        }

        //Initial population setup
        PerformInitialSpawning();

        //Start enviromental cycle
        
    }


    private void Update()
    {
        //Update game time with time scaling
        gameTime += Time.deltaTime * timeScale;

        //Check if it's time to update the population data
        if (gameTime - lastPopulationUpdateTime >= populationUpdateInterval)
        {
            UpdatePopulations();
            lastPopulationUpdateTime = gameTime; //Reset the last update time
        }
    }


    private void PerformInitialSpawning()
    {
        foreach (FishType fishType in managedFishTypes)
        {
            int initialCount = Mathf.RoundToInt(fishType.targetPopulation * populationMultiplier); //Spawn full multiplied target for more fish

            //Find suitable spawn region for the fish type
            List<SpawnRegion> suitableRegions = FindSuitableRegions(fishType);
            if (suitableRegions.Count > 0)
            {
                // Create multiple smaller schools rather than one big school
                // This prevents all fish being clustered in one location
                int numberOfSchools = Mathf.Max(3, initialCount / 8); // At least 3 schools, or more for larger populations
                
                // Calculate fish per school, ensuring some variation in school sizes
                List<int> schoolSizes = new List<int>();
                int remainingFish = initialCount;
                
                for (int i = 0; i < numberOfSchools && remainingFish > 0; i++)
                {
                    // Create naturally varied school sizes
                    int baseSize = remainingFish / (numberOfSchools - i);
                    int variation = Mathf.RoundToInt(baseSize * 0.4f); // Up to 40% variation in school size
                    int schoolSize = baseSize + UnityEngine.Random.Range(-variation, variation);
                    
                    // Ensure school size is at least 1 and doesn't exceed remaining fish
                    schoolSize = Mathf.Clamp(schoolSize, 1, remainingFish);
                    schoolSizes.Add(schoolSize);
                    remainingFish -= schoolSize;
                }
                
                // Spawn each school in a different location throughout the water body
                foreach (int schoolSize in schoolSizes)
                {
                    // Choose a random region for this school
                    SpawnRegion region = suitableRegions[UnityEngine.Random.Range(0, suitableRegions.Count)];
                    
                    // Generate a fish type with appropriate rarity based on size and position
                    SerializableFishItem serializableFishItem;
                    
                    // Create natural rarity distribution - rare/epic fish spawn less frequently
                    float rarityRoll = UnityEngine.Random.value;
                    if (rarityRoll < 0.6f) // 60% common or uncommon
                    {
                        // Smaller fish are more common
                        serializableFishItem = fishGenerator.GenerateFish(fishType, 1, 
                            UnityEngine.Random.value < 0.7f ? Rarity.Common : Rarity.Uncommon);
                    }
                    else if (rarityRoll < 0.9f) // 30% rare or medium
                    {
                        // Medium fish are less common
                        serializableFishItem = fishGenerator.GenerateFish(fishType, 1, Rarity.Rare);
                    }
                    else // 10% epic or legendary
                    {
                        // Large fish are rare
                        serializableFishItem = fishGenerator.GenerateFish(fishType, 1, 
                            UnityEngine.Random.value < 0.8f ? Rarity.Epic : Rarity.Legendary);
                    }
                    
                    // Spawn the fish group with this rarity and size
                    SpawnFishGroup(fishType, region, schoolSize, serializableFishItem);
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


    private void SpawnFishGroup(FishType fishType, SpawnRegion region, int count, SerializableFishItem serializableFishItem)
    {
        //Create a parent object for the school
        GameObject schoolParent = new GameObject(fishType.speciesID + " School");

        //Get population data
        PopulationData populationData = populations[fishType.speciesID];

        // Get the bounds of the entire scene rather than just the spawn region
        // This will allow fish to spread across the entire water body
        
        // First get the camera bounds to determine the full available width
        Camera mainCamera = Camera.main;
        float fullSceneWidth = 50f; // Default fallback width
        
        if (mainCamera != null)
        {
            // Calculate the full width of the scene based on camera's orthographic size
            float height = 2f * mainCamera.orthographicSize;
            float width = height * mainCamera.aspect;
            fullSceneWidth = width * 1.5f; // Extend beyond visible area
        }
        
        // Determine a weight factor for positioning (0-1 range)
        float weightFactor = UnityEngine.Random.value; // Random weight factor for this school
        
        // Position fish based on their weight, but across the full scene width
        Vector2 regionCenter = (Vector2)region.transform.position;
        
        // Generate position across entire width but respect the depth based on weight
        float xPos = UnityEngine.Random.Range(-fullSceneWidth/2, fullSceneWidth/2);
        float yPos = region.GetYPositionForWeightedDepth(weightFactor);
        
        Vector2 schoolCenter = new Vector2(xPos, yPos);
        schoolParent.transform.position = schoolCenter;

        for (int i = 0; i < count; i++)
        {
            // More natural school formations with proper variation
            // Fish in the same school should follow natural schooling patterns
            
            // Calculate formation position:
            // - Smaller variance for schooling fish
            // - Allow some stragglers
            bool isStraggler = UnityEngine.Random.value < 0.25f; // 25% chance of straggler for more natural distribution
            
            // Use polar coordinates for more natural, non-rectangular distribution
            float distance;
            float angle;
            
            if (isStraggler) {
                // Stragglers can be anywhere in the water body, much more spread out
                distance = UnityEngine.Random.Range(3.0f, 20.0f); // Much wider range
                angle = UnityEngine.Random.Range(0f, 360f); // Any direction
            } else {
                // School members have more natural, organic distribution
                // Natural school formations are elliptical, not rectangular
                distance = UnityEngine.Random.Range(0.5f, 5.0f); // Closer to school center
                angle = UnityEngine.Random.Range(0f, 360f); // Any direction but closer
            }
            
            // Convert polar to cartesian coordinates for more natural-looking distribution
            float xVariance = distance * Mathf.Cos(angle * Mathf.Deg2Rad);
            float yVariance = distance * Mathf.Sin(angle * Mathf.Deg2Rad) * 0.4f; // Flattened vertically
                
            Vector2 spawnPosition = schoolCenter + new Vector2(xVariance, yVariance);

            //Create fish instance
            //Get the FishGenerator component from the FishManager
            if (fishGenerator == null)
            {
                Debug.LogError("FishGenerator component not found on FishManager. Please add it.");
                return;
            }

            
            GameObject fishObj = Instantiate(fishType.prefab, spawnPosition, Quaternion.identity); //Instantiate the fish prefab
            fishObj.transform.parent = schoolParent.transform; //Set the parent to the school object

            //Set up fish AI
            FishAI fishAI = fishObj.GetComponent<FishAI>();
            if (fishAI == null)
            {
                fishAI = fishObj.AddComponent<FishAI>(); //Add FishAI component if not present
            }

            fishAI.Initialize(fishType, region, schoolCenter, serializableFishItem); //Initialize fish AI 

            //Subscribe to fish events
            fishAI.OnFishDeath += HandleFishDeath; //Subscribe to the death event
            fishAI.OnFishReproduce += HandleFishReproduction; //Subscribe to the reproduction event

            populationData.activeFish.Add(fishAI); //Add fish to the active list
            populationData.currentPopulation++; //Increment the current population

        }
        populationData.lastSpawnTime = gameTime; //Update the last spawn time for this fish type
    }


    private void UpdatePopulations()
    {
        foreach (var entry in populations)
        {
            PopulationData data = entry.Value;
            FishType fishType = data.fishType;

            data.activeFish.RemoveAll(fish => fish == null); //Remove any null fish references

            data.currentPopulation = data.activeFish.Count; //Update the current population count

            //Check if we need to spawn more fish
            int adjustedTargetPopulation = Mathf.RoundToInt(fishType.targetPopulation * populationMultiplier);
            if (data.currentPopulation < adjustedTargetPopulation)
            {
                //Calculate spawn probability based n time since last spawn
                float timeSinceLastSpawn = gameTime - data.lastSpawnTime;
                float spawnChance = timeSinceLastSpawn * fishType.spawnRate * timeScale; //Adjust spawn chance

                if (Random.value < spawnChance)
                {
                    int fishToSpawn = Mathf.Min(
                        Random.Range(2, 6), // Randomly spawn 2-5 fish at once for faster repopulation
                        adjustedTargetPopulation - data.currentPopulation // Ensure we don't exceed adjusted target population
                    );

                    if (fishToSpawn > 0)
                    {
                        //Find suitable regions for spawning
                        List<SpawnRegion> suitableRegions = FindSuitableRegions(fishType); //Find suitable regions for spawning
                        if (suitableRegions.Count > 0)
                        {
                            // Select a random region far from existing fish to prevent clustering
                            SpawnRegion region = suitableRegions[Random.Range(0, suitableRegions.Count)];
                            
                            // Create varied fish sizes with natural distribution
                            SerializableFishItem serializableFishItem;
                            
                            // Create natural size distribution based on depth and time
                            // This creates ecological diversity - different zones have different fish
                            float sizeRoll = UnityEngine.Random.value;
                            
                            // Deeper spawns tend to be larger fish
                            if (region.maxDepth > 12f && sizeRoll > 0.6f) {
                                // Deep water has chance for large fish
                                serializableFishItem = fishGenerator.GenerateFish(fishType, 1, 
                                    UnityEngine.Random.value < 0.7f ? Rarity.Epic : Rarity.Legendary);
                            }
                            else if (region.maxDepth > 8f && sizeRoll > 0.4f) {
                                // Medium depths have medium fish
                                serializableFishItem = fishGenerator.GenerateFish(fishType, 1, Rarity.Rare);
                            }
                            else {
                                // Shallow waters have smaller fish
                                serializableFishItem = fishGenerator.GenerateFish(fishType, 1, 
                                    UnityEngine.Random.value < 0.6f ? Rarity.Common : Rarity.Uncommon);
                            }
                            
                            SpawnFishGroup(fishType, region, fishToSpawn, serializableFishItem);
                        }
                    }
                }
            }
        }   
    }


    private void HandleFishDeath(FishAI fish)
    {
        if (fish == null || fish.fishType == null) return; //Exit if fish is null or fish type is not set

        string speciesID = fish.fishType.speciesID; //Get the species ID of the fish
        if (populations.TryGetValue(speciesID, out PopulationData populationData))
        {
            populationData.activeFish.Remove(fish); //Remove fish from the active list
            populationData.currentPopulation--; //Decrement the current population
        }
    }


    private void HandleFishReproduction(FishAI parent)
    {
        if (parent == null) return;

        string speciesID = parent.fishType.speciesID; //Get the species ID of the parent fish
        if (populations.TryGetValue(speciesID, out PopulationData populationData))
        {
            if (populationData.currentPopulation >= parent.fishType.maxPopulation)
            {
                //If the population is at max, don't allow reproduction
                return;
            }
            Vector2 parentPosition2D = new Vector2(parent.transform.position.x, parent.transform.position.y);
            Vector2 spawnPos = parentPosition2D + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)); //Random position around the parent fish
            GameObject babyFishObj = Instantiate(parent.fishType.prefab, spawnPos, Quaternion.identity); //Create a new fish object
   
            //Set up fish AI for the baby fish
            FishAI babyFishAI = babyFishObj.GetComponent<FishAI>();
            if (babyFishAI == null)
            {
                babyFishAI = babyFishObj.AddComponent<FishAI>(); //Add FishAI component if not present
            }

            babyFishAI.Initialize(parent.fishType, parent.currentRegion, parent.homePosition); //Initialize baby fish AI
            babyFishAI.OnFishDeath += HandleFishDeath; //Subscribe to the death event
            babyFishAI.OnFishReproduce += HandleFishReproduction; //Subscribe to the reproduction event

            babyFishAI.age = 0f; //Set the age of the baby fish to 0
            babyFishAI.transform.localScale = Vector3.one * 0.5f; //Scale down the baby fish

            //Track this fish in our populaiton
            populationData.activeFish.Add(babyFishAI); //Add baby fish to the active list
            populationData.currentPopulation++; //Increment the current population

        }
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
