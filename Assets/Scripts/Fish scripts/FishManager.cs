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


}
