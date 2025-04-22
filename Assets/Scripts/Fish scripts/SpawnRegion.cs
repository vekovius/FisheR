using UnityEngine;

[ExecuteAlways] //Allows code to execute in edit mode!
public class SpawnRegion : MonoBehaviour
{
    [Header("Region Dimensions")]
    public Vector2 size = new Vector2(10f, 5f);
    public float waterSurfaceY = 16f;
    public float minDepth = 0f;
    public float maxDepth = 10f;

    [Header("Region Properties")]
    public string regionID;

    [Header("Visualization")]
    public Color gizmoColor = Color.green; //Color for gizmo in edit

    public Bounds GetBounds()
    {
        return new Bounds(transform.position, size);
    }

    //Get the actual Y cord for a specific depth from the water surface
    public float GetYPositionForDepth(float depthFromSurface)
    {
        return waterSurfaceY - depthFromSurface;
    }

    public Vector2 GetRandomPosition()
    {
        Bounds bounds = GetBounds(); //Get the bounds of the spawn region
        
        return new Vector2(
            Random.Range(bounds.min.x, bounds.max.x), //Random X within the bounds
            Random.Range(GetYPositionForDepth(maxDepth), GetYPositionForDepth(minDepth)) //Random Y within the depth range
        );
    }
    
    // Returns only the Y position based on weight factor
    // This allows fish to be positioned across the entire scene width
    public float GetYPositionForWeightedDepth(float weightFactor)
    {
        // Get the FishType for this region
        FishType fishType = null;
        FishManager fishManager = FindObjectOfType<FishManager>();
        if (fishManager != null)
        {
            foreach (FishType type in fishManager.managedFishTypes)
            {
                if (type.speciesID == speciesID)
                {
                    fishType = type;
                    break;
                }
            }
        }
        
        float minDepthForWeight, maxDepthForWeight;
        
        // Determine depth range based on weight but with more natural distribution
        if (fishType != null)
        {
            // Use a smooth curve distribution across depths rather than sharp category divisions
            // This prevents the artificial-looking "layering" of fish
            
            if (weightFactor <= 0.33f) // Smaller fish
            {
                // Allow smaller fish throughout the water column with preference for middle depths
                // This is more natural as small fish can be anywhere with proper schooling
                minDepthForWeight = fishType.lightWeightDepthMin + UnityEngine.Random.Range(0f, 3.0f);
                maxDepthForWeight = fishType.lightWeightDepthMax + UnityEngine.Random.Range(0f, 2.0f);
                
                // Small random chance for small fish to be deeper or shallower
                if (UnityEngine.Random.value < 0.2f) {
                    // Sometimes small fish go deeper or shallower
                    if (UnityEngine.Random.value < 0.5f) {
                        // Go deeper
                        minDepthForWeight += UnityEngine.Random.Range(2.0f, 5.0f);
                        maxDepthForWeight += UnityEngine.Random.Range(2.0f, 5.0f);
                    } else {
                        // Go shallower but avoid surface
                        minDepthForWeight = Mathf.Max(2.0f, minDepthForWeight - UnityEngine.Random.Range(1.0f, 2.0f));
                        maxDepthForWeight = Mathf.Max(3.0f, maxDepthForWeight - UnityEngine.Random.Range(1.0f, 2.0f));
                    }
                }
            }
            else if (weightFactor <= 0.66f) // Medium fish
            {
                // Medium fish can be anywhere in middle depths
                minDepthForWeight = fishType.mediumWeightDepthMin + UnityEngine.Random.Range(-1.0f, 2.0f);
                maxDepthForWeight = fishType.mediumWeightDepthMax + UnityEngine.Random.Range(-1.0f, 2.0f);
                
                // Sometimes medium fish go deeper
                if (UnityEngine.Random.value < 0.3f) {
                    minDepthForWeight += UnityEngine.Random.Range(1.0f, 4.0f);
                    maxDepthForWeight += UnityEngine.Random.Range(1.0f, 4.0f);
                }
                
                // Ensure enough depth for medium fish
                minDepthForWeight = Mathf.Max(3.0f, minDepthForWeight);
            }
            else // Larger fish
            {
                // Larger fish tend toward deeper water, but with variability
                // Some should still be found in mid-depths
                minDepthForWeight = fishType.heavyWeightDepthMin + UnityEngine.Random.Range(-2.0f, 3.0f);
                maxDepthForWeight = fishType.heavyWeightDepthMax + UnityEngine.Random.Range(-2.0f, 2.0f);
                
                // Occasionally allow large fish in shallower water - this is natural
                if (UnityEngine.Random.value < 0.15f) {
                    minDepthForWeight = Mathf.Max(4.0f, minDepthForWeight - UnityEngine.Random.Range(3.0f, 6.0f));
                    maxDepthForWeight = Mathf.Max(6.0f, maxDepthForWeight - UnityEngine.Random.Range(3.0f, 6.0f));
                }
            }
            
            // Ensure depths are within region bounds
            minDepthForWeight = Mathf.Max(minDepthForWeight, minDepth);
            maxDepthForWeight = Mathf.Min(maxDepthForWeight, maxDepth);
        }
        else
        {
            // Fallback if fishType not found
            minDepthForWeight = minDepth;
            maxDepthForWeight = maxDepth;
        }
        
        // Return a random Y position between min and max depths
        return UnityEngine.Random.Range(
            GetYPositionForDepth(maxDepthForWeight), 
            GetYPositionForDepth(minDepthForWeight)
        );
    }
    
    // Existing method, but modified for better scene-wide distribution
    public Vector2 GetRandomPositionByWeight(float weightFactor)
    {
        // Get position across entire width but respect the depth based on weight
        float yPos = GetYPositionForWeightedDepth(weightFactor);
        
        // Get a random X position within this region's bounds
        Bounds bounds = GetBounds();
        float xPos = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        
        // Return the position
        return new Vector2(xPos, yPos);
    }

    public string speciesID; //SpeciesID
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(transform.position, size); //Draws wire cube, well square bc we passing 2d vector 
    }    
}
