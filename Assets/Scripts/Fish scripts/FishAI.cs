using System;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class FishAI : MonoBehaviour
{
    //Events for ecosystem management
    public event Action<FishAI> OnFishDeath; //Event for when fish dies
    public event Action<FishAI> OnFishReproduce; //Event for when fish reproduces

    public SerializableFishItem fishData; //Data for the fish, assigned by FishSpawner

    [Header("Fish Properties")]
    public FishType fishType; //Assigned by FishSpawner
    public SpawnRegion currentRegion; //Assigned by FishSpawner
    public Vector2 homePosition;
    public float age = 0f; //Age of the fish in minutes
    public float health = 1f; //Health of the fish, 1 is full health, 0 is dead
    public float hunger = 0f; //Hunger of the fish, 0 is full, 1 is starving
    public float maturity = 0f; //Maturity of the fish, 0 is immature, 1 is mature

    [Header("Navigational Properties")]
    public Vector2 velocity;
    public Transform currentLure = null;
    public float lureAttractionRadius = 5f; //Radius for lure attraction, fish will be attracted to lures within this radius
    public Transform min, max;

    //Components
    private SpriteRenderer spriteRenderer;
    Rigidbody2D rb;

    public void Initialize(FishType type, SpawnRegion region, Vector2 home, SerializableFishItem fishData = null)
    {
        fishType = type;
        currentRegion = region;
        homePosition = home;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize with purely horizontal movement - fish swim side to side naturally
        // Fish should start with consistent lateral movement within the school
        velocity = new Vector2(
            UnityEngine.Random.Range(-0.7f, 0.7f), 
            // Almost no vertical component in initial velocity
            0);

        lureAttractionRadius = fishType.lureAttractionRadius; //Set the lure attraction radius to the same as the neighbor radius 

        if (fishData != null)
        {
            this.fishData = fishData;
            ApplyFishData();
        }
        
        // Apply size variations immediately for visual diversity
        if (this.fishData != null && this.fishData.sizeMultiplier > 0)
        {
            // Apply the size multiplier directly to the fish scale
            transform.localScale = Vector3.one * this.fishData.sizeMultiplier;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        min = GameObject.FindGameObjectWithTag("WaterBottomLeft").transform;
        max = GameObject.FindGameObjectWithTag("WaterTopRight").transform;

        Initialize(fishType, currentRegion, homePosition); //Initialize fish properties
        if (fishType == null)
        {
            Debug.LogError("FishType is not assigned in FishAI. Please assign a FishType in the inspector.");
            return;
        }
    }

    private void FixedUpdate()
    {
        Vector2 acceleration = Vector2.zero;
        acceleration += Flock();
        acceleration += Wander();
        acceleration += HomeAttraction();
        acceleration += lureAttraction();

        velocity += acceleration * Time.fixedDeltaTime;
        velocity = Vector2.ClampMagnitude(velocity, fishType.maxSpeed);

        // Get water level from any existing StateController
        float waterLevel = float.MaxValue;
        StateController stateController = FindObjectOfType<StateController>();
        if (stateController != null)
        {
            waterLevel = stateController.waterLevel;
        }

        // Calculate new position
        Vector2 newPosition = rb.position + velocity * Time.fixedDeltaTime;
        
        // More natural surface interaction with occasional surface swimming and jumping
        float fishSize = transform.localScale.x;
        
        // Large fish rarely go to surface, but small fish might occasionally
        bool canBreachSurface = (fishSize < 0.3f && UnityEngine.Random.value < 0.001f);
        
        // Most fish avoid the surface but don't have hard boundaries - this is more natural
        float surfaceAvoidanceStrength;
        float naturalDepthPreference;
        
        // Size-based preferences (not hard constraints)
        if (fishSize > 0.65f) {
            // Large fish prefer deep water but can occasionally swim higher
            naturalDepthPreference = UnityEngine.Random.Range(6.0f, 14.0f);
            surfaceAvoidanceStrength = 0.95f; // Strong preference to avoid surface
        } else if (fishSize > 0.4f) {
            // Medium fish prefer mid-depths but have wide range
            naturalDepthPreference = UnityEngine.Random.Range(4.0f, 10.0f);
            surfaceAvoidanceStrength = 0.85f; // Moderate preference to avoid surface
        } else {
            // Small fish can be throughout column with some preference for mid depths
            naturalDepthPreference = UnityEngine.Random.Range(2.0f, 7.0f);
            surfaceAvoidanceStrength = 0.75f; // Mild preference to avoid surface
        }
        
        // Calculate distance from surface
        float distanceFromSurface = waterLevel - newPosition.y;
        
        // Apply natural surface avoidance without hard boundaries
        if (distanceFromSurface < 0.5f && !canBreachSurface)
        {
            // Too close to surface - apply stronger avoidance
            velocity.y = -Mathf.Abs(velocity.y) * 0.8f - 0.3f;
            
            // Add slightly random horizontal movement when avoiding surface
            velocity.x += UnityEngine.Random.Range(-0.2f, 0.2f);
        }
        else if (distanceFromSurface < naturalDepthPreference)
        {
            // Gradually increase downward tendency as fish gets closer to surface
            // This creates smooth, natural depth-seeking behavior rather than hard boundaries
            float surfaceFactor = 1.0f - (distanceFromSurface / naturalDepthPreference);
            
            // Apply gentle downward force proportional to how close to surface
            // This is not absolute - just a preference that gets stronger near surface
            velocity.y -= surfaceFactor * surfaceAvoidanceStrength * 0.1f * UnityEngine.Random.Range(0.6f, 1.4f);
        }
        
        // Allow occasional surface swimming or jumping for small fish
        if (canBreachSurface && distanceFromSurface < 1.0f)
        {
            // Small chance for small fish to briefly break the surface (jumping)
            velocity.y = UnityEngine.Random.Range(0.5f, 1.5f);
        }
        
        // Allow fish to swim across the entire scene width, not just their spawn region
        // This distributes fish across the entire game area
        
        // Calculate scene boundaries based on camera
        float sceneWidth = 100f; // Large default value
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            float height = 2f * mainCamera.orthographicSize;
            float width = height * mainCamera.aspect;
            sceneWidth = width * 2.0f; // Much wider than visible area
        }
        
        // Only constrain X position at the far edges of the scene, not region boundaries
        float worldMinX = min.position.x;
        float worldMaxX = max.position.x;
        
        if (newPosition.x < worldMinX) 
        {
            newPosition.x = worldMinX;
            velocity.x = -velocity.x * 0.5f;
        }
        else if (newPosition.x > worldMaxX) 
        {
            newPosition.x = worldMaxX;
            velocity.x = -velocity.x * 0.5f;
        }


        float worldMinY = min.position.y;
        float worldMaxY = max.position.y;

        //float minY = currentRegion.GetYPositionForDepth(currentRegion.maxDepth);
        if (newPosition.y < worldMinY)
        {
            newPosition.y = worldMinY;
            velocity.y = -velocity.y * 0.5f;
        }
        else if (newPosition.y > worldMaxY)
        {
            newPosition.y = worldMaxY;
            velocity.y = -velocity.y * 0.5f;
        }


        rb.MovePosition(newPosition);

        //Set the fish rotation to match the direction of movement
        if (velocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0,0,angle);
        }

        //Set the direction of the fish to always be rightside up
        if (velocity.x < 0)
        {
            spriteRenderer.flipY = true;
        }
        else
        {
            spriteRenderer.flipY = false;
        }
    }

    private void Update()
    {
        UpdateLifeCycle(); //Update the life cycle of the fish
        CheckReproduction(); //Check if the fish can reproduce
    }

    private void ApplyFishData()
    {
        if (fishData == null) return;

        //Apply modifiers
        fishType.maxSpeed *= fishData.speedMultiplier;
        
        // Set the fish size directly based on the size multiplier
        // This ensures fish have visible size differences
        transform.localScale = Vector3.one * fishData.sizeMultiplier;
    }

    // Natural fish schooling behaviors vary by species and situation
    private Vector2 Flock()
    {
        // More biologically accurate schooling algorithm:
        // 1. Fish align with neighbors of same size/species
        // 2. Fish maintain optimal distance (not too close, not too far)
        // 3. Different behaviors based on fish size & situation
        
        //Collections all neighbors within neighbor Radius
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, fishType.neighborRadius);
        Vector2 alignment = Vector2.zero;
        Vector2 cohesion = Vector2.zero;
        Vector2 separation = Vector2.zero;
        int count = 0;

        // Fish of similar size
        int similarSizedFishCount = 0;
        
        Vector2 forward = velocity; //Get the forward direction of the fish
        if (velocity.sqrMagnitude < 0.1f) //If the fish is not moving, set forward to a default direction
        {
            forward = Vector2.right;
        }

        // Check for lure presence - fish should disperse more when lure is present
        GameObject lure = GameObject.FindWithTag("Lure") ?? GameObject.FindWithTag("OccupiedLure");
        bool lureIsPresent = lure != null;
        bool lureIsOccupied = lure != null && lure.CompareTag("OccupiedLure");
        
        // Various factors affecting school behavior
        float dispersalMultiplier;
        if (lureIsOccupied) {
            dispersalMultiplier = 5.0f; // Dramatic dispersal when a fish is caught
        } else if (lureIsPresent) {
            dispersalMultiplier = 2.0f; // Moderate dispersal when lure is present but no fish caught
        } else {
            dispersalMultiplier = 1.0f; // Normal schooling when no lure
        }
        
        // Increase separation distance when lure is present to reduce clumping
        float effectiveSeparationDistance = fishType.separationDistance * dispersalMultiplier;
        
        // Track my own size for comparison
        float mySize = transform.localScale.x;

        //Goes though all neighbors within range
        foreach (Collider2D col in neighbors)
        {
            if (col.gameObject == gameObject)
                continue;

            FishAI other = col.GetComponent<FishAI>();
            
            if (other != null)
            {
                //Calculate the direction vector to the neighbor fish
                Vector2 dirToNeighbor = other.transform.position - transform.position;

                //Check if neighbor is within the field of view
                float angleToNeighbor = Vector2.Angle(forward, dirToNeighbor);
                if (angleToNeighbor > fishType.fieldOfView * 0.5f)
                {
                    continue;
                }
                
                // Natural schooling behaviors based on size similarity
                float neighborSize = other.transform.localScale.x;
                float sizeDifference = Mathf.Abs(mySize - neighborSize);
                
                // Is this fish similar to me? (within 30% of my size)
                bool isSimilarSize = sizeDifference < (mySize * 0.3f);
                
                // Natural schooling - fish prefer to align with similar sized fish
                if (isSimilarSize)
                {
                    similarSizedFishCount++;
                }
                
                // Size-based flocking multipliers - small fish follow bigger fish
                // but big fish ignore smaller ones
                float alignmentMultiplier = isSimilarSize ? 1.0f : (neighborSize > mySize ? 0.8f : 0.3f);
                float cohesionMultiplier = isSimilarSize ? 1.0f : (neighborSize > mySize ? 0.7f : 0.4f);
                
                // Smaller fish maintain more distance from bigger fish
                float sizeBasedSeparation = neighborSize > mySize ? 1.5f : 1.0f;

                // Apply stronger separation to prevent clumping
                if (dirToNeighbor.magnitude < effectiveSeparationDistance) 
                {
                    // Increase separation force as fish get closer to each other
                    float separationForce = 1.0f - (dirToNeighbor.magnitude / effectiveSeparationDistance);
                    separationForce = Mathf.Pow(separationForce, 2) * dispersalMultiplier; // Square the force for stronger effect at close distances
                    
                    // Size-based separation - smaller fish avoid larger ones more
                    separationForce *= sizeBasedSeparation;
                    
                    separation -= dirToNeighbor.normalized * separationForce * 4.0f;
                    
                    // Prioritize horizontal movement when separating
                    // Real fish separate by swimming horizontally, not vertically
                    Vector2 horizontalSeparation = dirToNeighbor;
                    horizontalSeparation.y = 0; // Remove vertical component
                    
                    // Add stronger horizontal separation
                    separation -= horizontalSeparation.normalized * separationForce * 2.0f;
                }

                // When lure is present or a fish is caught, drastically alter schooling behavior
                float alignmentFactor, cohesionFactor;
                
                if (lureIsOccupied) {
                    // Fish scatter when one is caught
                    alignmentFactor = 0.1f; // Almost no alignment - panic!
                    cohesionFactor = 0.0f;  // No cohesion - every fish for itself!
                } else if (lureIsPresent) {
                    // More cautious schooling when lure is present
                    alignmentFactor = 0.4f;
                    cohesionFactor = 0.3f;
                } else {
                    // Normal schooling
                    alignmentFactor = 1.0f;
                    cohesionFactor = 1.0f;
                }
                
                // Apply all modifiers to create natural schooling
                alignment += other.velocity * alignmentFactor * alignmentMultiplier;
                cohesion += (Vector2)other.transform.position * cohesionFactor * cohesionMultiplier;
                
                count++;
            }
        }
        
        // Different behavior based on how many fish of similar size are nearby
        // This creates natural schooling by size - real fish school by size
        if (count > 0)
        {
            // Calculate flocking forces with horizontal emphasis
            alignment = (alignment / count).normalized * fishType.maxSpeed - velocity;
            // Reduce vertical component of alignment
            alignment.y *= 0.2f;
            
            // Cohesion calculation - emphasize horizontal movement
            Vector2 cohesionCenter = (cohesion / count);
            Vector2 toCohesionCenter = cohesionCenter - (Vector2)transform.position;
            // Reduce vertical component of cohesion by 80%
            toCohesionCenter.y *= 0.2f;
            cohesion = toCohesionCenter.normalized * fishType.maxSpeed - velocity;
            
            // Apply separation based on situation
            float sepMultiplier = 1.5f;
            
            // Adjust separation strength based on situation
            if (lureIsOccupied) {
                // Maximum separation when fish are fleeing
                sepMultiplier = 5.0f;
            } else if (lureIsPresent) {
                // High separation when cautious
                sepMultiplier = 3.0f;
            } else if (similarSizedFishCount >= 3) {
                // In a proper school, slightly less separation
                sepMultiplier = 1.2f;
            }
            
            separation = separation.normalized * fishType.maxSpeed * sepMultiplier - velocity;
            
            // Add minimal downward force to prevent surface swimming
            // But keep fish swimming mostly horizontally
            float surfaceAvoidanceForce = 0.3f; // Significantly reduced vertical force
            
            // Larger fish stay deeper, but still move mostly horizontally
            if (transform.localScale.x > 0.6f) {
                surfaceAvoidanceForce = 0.5f;
            }
            
            // Apply minimal downward tendency, but prioritize horizontal movement
            separation.y -= surfaceAvoidanceForce;
        }
        
        // Natural random variations
        // Each fish should have slightly different behavior
        
        // Individual temperament variation
        // Real fish have individual behaviors even within the same species
        float individualTemperament = UnityEngine.Random.Range(0.0f, 1.0f);
        
        // Temperament affects weights - some fish are more independent, others more social
        float alignmentVariation = Mathf.Lerp(0.7f, 1.3f, individualTemperament);
        float cohesionVariation = UnityEngine.Random.Range(0.8f, 1.2f);
        float separationVariation = Mathf.Lerp(1.3f, 0.9f, individualTemperament); 
        
        // Small random movement variations
        float randomVariation = UnityEngine.Random.value;
        if (randomVariation < 0.05f) {
            // Occasional random darting/turning - real fish do this
            alignmentVariation *= 0.5f;
            separationVariation *= 1.5f;
        }

        // Final combined movement forces
        return alignment * (fishType.alignmentWeight * alignmentVariation) + 
               cohesion * (fishType.cohesionWeight * cohesionVariation) + 
               separation * (fishType.separationWeight * separationVariation);
    }


    //Source of random direction for fish with realistic movement patterns
    private Vector2 Wander()
    {
        // Size-based movement patterns - larger fish move more deliberately
        float fishSize = transform.localScale.x;
        float horizontalWander;
        float verticalWander = 0;
        
        // Make burst probability and movement style depend on fish size
        float burstProbability;
        float movementChange;
        
        // Tiny fish (minnows) - extremely quick, schooling movements
        if (fishSize <= 0.25f) {
            // Tiny fish make quick darting movements in tight schools
            horizontalWander = UnityEngine.Random.Range(-1.0f, 1.0f); // Fast movement
            
            // Tiny fish change direction very frequently in unison
            movementChange = 0.08f; // Very frequent direction changes
            burstProbability = 0.1f; // Frequent quick bursts
            
            // Tiny fish make small vertical adjustments regularly
            if (UnityEngine.Random.value < 0.05f) {
                verticalWander = UnityEngine.Random.Range(-0.1f, 0.1f); // More vertical movement
            }
        }
        // Small fish (bluegill, small perch) - quick, agile movements
        else if (fishSize <= 0.4f) {
            // Small fish make quick movements but with more purpose
            horizontalWander = UnityEngine.Random.Range(-0.9f, 0.9f); // Quick movement
            
            // Small fish change direction frequently
            movementChange = 0.05f; // Frequent direction changes
            burstProbability = 0.08f; // Frequent bursts
            
            // Some vertical adjustment
            if (UnityEngine.Random.value < 0.03f) {
                verticalWander = UnityEngine.Random.Range(-0.08f, 0.08f); // Moderate vertical movement
            }
        }
        // Medium fish (trout, bass) - balanced movements
        else if (fishSize <= 0.55f) {
            // Medium fish have moderate, purposeful movements
            horizontalWander = UnityEngine.Random.Range(-0.7f, 0.7f); // Moderate movement
            
            // Medium fish change direction somewhat frequently
            movementChange = 0.03f;
            burstProbability = 0.05f; // Occasional bursts
            
            // Some vertical adjustment
            if (UnityEngine.Random.value < 0.02f) {
                verticalWander = UnityEngine.Random.Range(-0.05f, 0.05f); // Some vertical adjustments
            }
        }
        // Large fish (big bass, pike) - slow, deliberate movements
        else {
            // Large fish move more slowly and deliberately
            horizontalWander = UnityEngine.Random.Range(-0.4f, 0.4f); // Slower movement
            
            // Large fish rarely change direction quickly
            movementChange = 0.01f; // Very rare direction changes
            burstProbability = 0.02f; // Rare bursts
            
            // Maintain depth with minimal vertical movement
            if (UnityEngine.Random.value < 0.01f) {
                verticalWander = UnityEngine.Random.Range(-0.03f, 0.03f); // Almost no vertical movement
            }
        }
        
        // Make direction changes less frequent to reduce erratic movement
        // Only change direction occasionally based on fish size
        if (UnityEngine.Random.value >= movementChange) {
            // Maintain previous movement direction with slight adjustments
            // This creates smooth, natural-looking movement
            if (velocity.magnitude > 0.1f) {
                horizontalWander = velocity.x * 0.95f + horizontalWander * 0.05f;
                verticalWander = velocity.y * 0.95f + verticalWander * 0.05f;
            }
        }
        
        // Add periodic behavior - sometimes fish make small "bursts" of movement
        if (UnityEngine.Random.value < burstProbability) 
        {
            // Direction-consistent bursts (avoid sudden reversals)
            if (horizontalWander * velocity.x >= 0) { // If going same direction
                horizontalWander *= 2.0f; // Occasional bursts of speed
            }
        }
        
        Vector2 wanderForce = new Vector2(horizontalWander, verticalWander);
        wanderForce *= fishType.wanderWeight;
        return wanderForce;
    }

    //This function keeps the fishies near their home point. 
    private Vector2 HomeAttraction()
    {
        // Get current depth and fish size for natural behavior
        float fishSize = transform.localScale.x;
        StateController stateController = FindObjectOfType<StateController>();
        float currentDepth = 0;
        
        if (stateController != null) {
            // Calculate current depth from water surface
            currentDepth = stateController.waterLevel - transform.position.y;
        }
        
        // Different territorial behaviors based on size and depth
        float territorialFactor;
        
        // Large fish are more territorial and stay in their home areas
        if (fishSize > 0.6f) {
            territorialFactor = 0.15f; // Strong home area preference
        }
        // Medium fish have moderate territory
        else if (fishSize > 0.4f) {
            territorialFactor = 0.08f; // Moderate home area preference
        }
        // Small fish have loose territories and school more
        else {
            territorialFactor = 0.03f; // Loose home area preference - more schooling
        }
        
        // Fish at different depths have different territory sizes
        // Deep water fish patrol larger territories
        if (currentDepth > 10f) {
            territorialFactor *= 0.7f; // Larger territory for deep fish
        }
        
        Vector2 toHome = (homePosition - (Vector2)transform.position).normalized * fishType.maxSpeed - velocity;
        toHome *= territorialFactor;
        return toHome;
    }

    private Vector2 lureAttraction()
    {
        if (currentLure == null)
        {
            GameObject lureObject = GameObject.FindWithTag("Lure");
            if (lureObject != null)
            {
                currentLure = lureObject.transform;
            }
        }
        else
        {
            // If a lure becomes occupied, it's being reeled in with a fish
            // This should scare surrounding fish
            if (currentLure.gameObject.CompareTag("OccupiedLure"))
            {
                // Create a fleeing response from hooked fish
                // Other fish should rapidly swim away from occupied lure
                Vector2 fleeDirection = ((Vector2)transform.position - (Vector2)currentLure.position).normalized;
                float fleeDistance = Vector2.Distance((Vector2)transform.position, (Vector2)currentLure.position);
                
                // Flee radius increased to 12 units - fish are more aware of caught fish
                if (fleeDistance < 12.0f)
                {
                    // Much stronger flee response when closer to the hooked fish
                    float fleeFactor = Mathf.Lerp(3.0f, 1.0f, fleeDistance / 12.0f);
                    
                    // Add downward component for more natural flight behavior
                    fleeDirection.y -= 0.3f;
                    
                    // Add random lateral movement to create scattering effect
                    fleeDirection.x += UnityEngine.Random.Range(-0.5f, 0.5f);
                    
                    // Return a much stronger fleeing impulse for dramatic scattering
                    return fleeDirection * fishType.maxSpeed * fleeFactor * 3.0f;
                }
                
                // Reset lure tracking
                currentLure = null;
                return Vector2.zero;
            }
        }

        if (currentLure != null)
        {
            float distanceTolure = Vector2.Distance((Vector2)transform.position, (Vector2)currentLure.position);

            // Only react to lure when relatively close, and don't always react
            float reactionThreshold = lureAttractionRadius * 0.6f; // Reduced detection range
            
            if (distanceTolure <= reactionThreshold)
            {
                // More variable lure reaction based on fish size
                // Larger fish are more cautious, smaller fish are more eager
                float ignoreProbability = 0.3f;
                if (transform.localScale.x > 0.6f) {
                    // Larger fish are more cautious and more likely to ignore
                    ignoreProbability = 0.5f;
                } else if (transform.localScale.x < 0.4f) {
                    // Smaller fish are more eager and less likely to ignore
                    ignoreProbability = 0.15f;
                }
                
                // Chance to ignore lure completely - fish don't always react to lures
                if (UnityEngine.Random.value < ignoreProbability)
                {
                    return Vector2.zero;
                }
                
                // Natural lure attraction with more varied behavior
                // Fish will show more diverse reactions - some quickly dart at lure, others approach cautiously
                float optimalDistance = reactionThreshold * 0.4f; // Most attraction at 40% of detection radius
                float attractionCurve = 1.0f - Mathf.Abs((distanceTolure - optimalDistance) / reactionThreshold);
                
                // More aggressive attraction variation
                float baseAttraction = UnityEngine.Random.Range(0.7f, 2.5f); // Much more variation in interest level
                float attractionStrength = Mathf.Lerp(0.1f, baseAttraction, attractionCurve);
                
                // More frequent and varied inspection behavior
                // Real fish often pause, dart forward, then back away from lures repeatedly
                if (UnityEngine.Random.value < 0.35f && distanceTolure < reactionThreshold * 0.6f)
                {
                    // More varied inspection behavior
                    if (UnityEngine.Random.value < 0.5f) {
                        // Sometimes pause completely
                        attractionStrength *= 0.1f;
                    } else {
                        // Sometimes back away slightly
                        attractionStrength *= -0.3f;
                    }
                }
                
                // Calculate direction to lure with minimal vertical component - more horizontal approach
                Vector2 lureDifference = (Vector2)currentLure.position - (Vector2)transform.position;
                Vector2 toLure = lureDifference.normalized;
                
                // Reduce vertical component of attraction - fish mostly approach horizontally
                toLure.y *= 0.3f;
                toLure = toLure.normalized * fishType.maxSpeed - velocity;
                
                return toLure * attractionStrength;
            }
        }
        return Vector2.zero;
    }

    private void UpdateLifeCycle()
    {
        //Increment age
        age += Time.deltaTime / 60f; //Convert to minutes

        maturity = Mathf.Clamp01(age / fishType.maturityAge); //Clamp maturity between 0 and 1

        hunger += fishType.hungerRate * Time.deltaTime / 60f; //Increment hunger based on hunger rate
        
        // Only adjust scale if no fish data is present (default fish behavior)
        if (fishData == null || fishData.sizeMultiplier <= 0)
        {
            float baseScale = 0.3f; // Smaller base size for better contrast
            float maxScale = 0.6f;  // Larger max size for more noticeable growth
            transform.localScale = Vector3.Lerp(Vector3.one * baseScale, Vector3.one * maxScale, maturity);
        }
        else
        {
            // For fish with data, use size directly from fish data
            // Add very slight size variation with maturity for natural growth appearance
            float maturityBonus = maturity * 0.1f; // Up to 10% size increase with maturity
            transform.localScale = Vector3.one * (fishData.sizeMultiplier + maturityBonus);
        }

        if (age > fishType.maxAge) //If fish is older than max age, die
        {
            Die();
        }
        
        // Apply gentle depth preferences instead of hard constraints
        // This is more natural as real fish have preferences but can still swim anywhere
        StateController stateController = FindObjectOfType<StateController>();
        if (stateController != null)
        {
            float waterLevel = stateController.waterLevel;
            float fishSize = transform.localScale.x;
            
            // Current depth from surface
            float currentDepthFromSurface = waterLevel - transform.position.y;
            
            // Fish that are extremely close to surface should have some natural tendency to go deeper
            // But not hard constraints - use extremely gradual corrections
            if (currentDepthFromSurface < 0.5f) 
            {
                // Only apply corrections 20% of the time to avoid uniform behavior
                if (UnityEngine.Random.value < 0.2f)
                {
                    // Add small downward nudge - doesn't force position
                    Vector2 nudge = Vector2.down * UnityEngine.Random.Range(0.05f, 0.2f);
                    
                    // Stronger for larger fish (which shouldn't be at surface)
                    nudge *= Mathf.Lerp(0.5f, 2.0f, fishSize);
                    
                    if (rb != null) {
                        rb.AddForce(nudge, ForceMode2D.Force);
                    }
                }
            }
            
            // Occasional natural depth-seeking behavior - spread across entire water column
            if (UnityEngine.Random.value < 0.01f) // Very occasional depth adjustment
            {
                float naturalDepthPreference;
                
                // Different size fish have preferences, but not hard boundaries
                if (fishSize > 0.6f) {
                    // Large fish tend to prefer deeper water but can venture anywhere
                    naturalDepthPreference = UnityEngine.Random.Range(6.0f, 12.0f);
                }
                else if (fishSize > 0.4f) {
                    // Medium fish tend to prefer middle depths
                    naturalDepthPreference = UnityEngine.Random.Range(4.0f, 8.0f);
                }
                else {
                    // Small fish can be throughout the column, but tend to school in mid-depths
                    naturalDepthPreference = UnityEngine.Random.Range(2.0f, 6.0f);
                }
                
                // If significantly away from preferred depth, make very subtle adjustment
                // This creates natural distribution without obvious layering
                float depthDifference = naturalDepthPreference - currentDepthFromSurface;
                
                // Apply very gentle correction that doesn't override behavior systems
                // Just a subtle tendency to drift toward natural depth ranges
                if (Mathf.Abs(depthDifference) > 3.0f) {
                    // Direction of adjustment based on whether too deep or too shallow
                    Vector2 depthAdjustment = (depthDifference > 0) ? Vector2.down : Vector2.up;
                    
                    // Extremely subtle force - just a tendency, not a constraint
                    depthAdjustment *= 0.05f;
                    
                    // Apply the gentle force
                    if (rb != null) {
                        rb.AddForce(depthAdjustment, ForceMode2D.Force);
                    }
                }
            }
        }
    }

    //Function to handle fish reproduction
    public void CheckReproduction()
    {
       
        if (maturity >= 1f) //If fish is mature
        {
            float reproductionChance = fishType.reproductionRate; //Calculate chance of reproduction
           
            if (UnityEngine.Random.value < reproductionChance) //If chance is met
            {
                OnFishReproduce?.Invoke(this); //Invoke the reproduction event
            }
        }
    }


    public void Die()
    {
        OnFishDeath?.Invoke(this); //Invoke the death event
        Destroy(gameObject); //Destroy the fish object
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, fishType.neighborRadius); //Draws white circle around neigbor seeing radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fishType.separationDistance); //Draws red circle around separtion seeing radius
    }

}
