using UnityEngine;
using UnityEngine.UI;

public class HookedState : StateInterface
{
    private GameObject lure;
    private GameObject hookedFish;
    private Rigidbody2D fishRb;
    private float struggleForce = 12f;          // Drastically increased struggle force - much harder
    private float playerForce = 40f;            // Further reduced player force - much more skill needed
    
    private GameObject tensionBarGameObject;
    private Image tensionBarMask;
    private float currentTension = 0f;
    private float maxTension = 100f;
    public static float tensionDecreaseRate = 15f;    // Much higher tension decay - significantly more challenge
    private float tensionIncreaseRate = 38f;    // Higher tension buildup - high risk/reward
    private float tensionDecayPause = 0.3f;     // Very short pause - requires excellent timing
    private float lastReelTime = 0f;            // Track when player last reeled
    
    // Rapid downshift penalty system
    private float lastDownshiftTime = -10f;     // Time of last downshift (initialized to negative for first use)
    private float downshiftCooldown = 0.6f;     // Further reduced cooldown for easier fishing (was 0.8f)
    private float rapidDownshiftPenalty = 20f;  // Further reduced penalty (was 25f)
    private int consecutiveDownshifts = 0;      // Count of rapid downshifts to increase penalty
    private bool cooldownVisualActive = false;  // Tracks whether cooldown visual is currently displayed

    // Sweet spot parameters - slightly more forgiving
    private float sweetSpotMin = 32f;           // Lowered for easier access (was 35f)
    private float sweetSpotMax = 58f;           // Increased for wider window (was 55f) 
    private float sweetSpotBonus = 1.7f;        // Slightly increased for better progress (was 1.6f)
    private float bonusPullTimer = 0f;          // Track time in sweet spot
    
    private float fishStruggleInterval = 1.2f;  // Even more frequent struggles
    private float nextStruggleTime = 0f;
    private float struggleVariability = 2.0f;   // Highly variable struggles - very unpredictable
    
    private float captureDistance = 2.0f;       // Further reduced capture distance - much more reeling required

    public HookedState(GameObject tensionBarGameObject = null)
    {
        this.tensionBarGameObject = tensionBarGameObject;
    }

    public void Enter()
    {
        lure = GameObject.FindWithTag("Lure");
        if (lure == null)
        {
            Debug.LogError("No lure found in HookedState");
            return;
        }

        // Find the hooked fish by checking for fish near the lure
        FindHookedFish();

        SetupTensionBar();

        // Disable fish AI behavior while hooked
        if (hookedFish != null)
        {
            FishAI fishAI = hookedFish.GetComponent<FishAI>();
            if (fishAI != null)
            {
                fishAI.enabled = false;
            }

            fishRb = hookedFish.GetComponent<Rigidbody2D>();
            if (fishRb == null)
            {
                Debug.LogError("Hooked fish has no Rigidbody2D component");
            }

            // Mark the lure as occupied with this fish
            LureStateController lureState = lure.GetComponent<LureStateController>();
            if (lureState == null)
            {
                // Add the component if it doesn't exist
                lureState = lure.AddComponent<LureStateController>();
            }
            lureState.SetOccupied(hookedFish);
        }
    }

    public void Update()
    {
        if (hookedFish == null || lure == null) return;

        // Make fish struggle (rotate and apply force away from player)
        ApplyFishStruggle();

        // Handle player input to reel in the fish
        HandlePlayerInput();

        // Update tension bar
        UpdateTensionBar();

        // Check if fish is caught or escaped
        CheckFishStatus();
    }

    public void Exit()
    {
        if (tensionBarGameObject != null)
        {
            tensionBarGameObject.SetActive(false);
        }
    }

    private void FindHookedFish()
    {
        // Find the closest fish to the lure
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(lure.transform.position, 1f);

        foreach (Collider2D collider in nearbyColliders)
        {
            if (collider.gameObject.GetComponent<FishAI>() != null)
            {
                hookedFish = collider.gameObject;
              
                // Attach the lure to the fish using a fixed joint
                AttachLureToFish();
                break;
            }
        }

        if (hookedFish == null)
        {
            Debug.LogError("No fish found near lure in HookedState");
        }
    }

    // Method to physically attach lure to the fish
    private void AttachLureToFish()
    {
        if (hookedFish == null || lure == null) return;

        // Get hook position
        Vector2 hookPosition = hookedFish.transform.position;


        // Create a fixed joint between the fish and the lure
        FixedJoint2D joint = hookedFish.AddComponent<FixedJoint2D>();
        joint.connectedBody = lure.GetComponent<Rigidbody2D>();
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = Vector2.zero; // Center of the fish

        // Disable the lure's collider to prevent further fish interactions
        Collider2D lureCollider = lure.GetComponent<Collider2D>();
        if (lureCollider != null)
        {
            lureCollider.enabled = false;
        }
    }

    private void SetupTensionBar()
    {
        if (tensionBarGameObject != null)
        {
            tensionBarGameObject.SetActive(true);

            Transform maskTransform = tensionBarGameObject.transform.Find("Mask");
            if (maskTransform != null)
            {
                tensionBarMask = maskTransform.GetComponent<Image>();
                currentTension = 0f;

            }

            if (tensionBarMask == null)
            {
                Debug.LogError("Tension bar mask image not found");
            }
        }
        else
        {
            Debug.Log("No tension bar assigned to HookedState");
        }
    }

    private void ApplyFishStruggle()
    {
        if (fishRb == null) return;

        // Find player position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // Calculate direction away from player
        Vector2 awayFromPlayer = (hookedFish.transform.position - player.transform.position).normalized;

        // Rotate fish to face away from player
        float angle = Mathf.Atan2(awayFromPlayer.y, awayFromPlayer.x) * Mathf.Rad2Deg;
        hookedFish.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Add constant smaller resistance force
        float constantResistance = struggleForce * 0.2f;
        fishRb.AddForce(awayFromPlayer * constantResistance * Time.deltaTime, ForceMode2D.Force);

        // Check if it's time for the fish to struggle
        if (Time.time > nextStruggleTime)
        {
            // Base struggle strength on fish size and rarity
            float struggleStrength = struggleForce;
            
            // Check if we have fish data for size-based difficulty
            FishAI fishAI = hookedFish.GetComponent<FishAI>();
            if (fishAI != null && fishAI.fishData != null)
            {
                // Bigger fish struggle harder
                if (fishAI.fishData.sizeMultiplier > 0)
                {
                    struggleStrength *= Mathf.Lerp(0.8f, 1.8f, Mathf.Clamp01((fishAI.fishData.sizeMultiplier - 0.4f) / 0.6f));
                }
                
                // Rarer fish struggle more
                if (fishAI.fishData.rarity > Rarity.Common)
                {
                    // Bonus struggle based on rarity
                    struggleStrength *= (1.0f + (float)fishAI.fishData.rarity * 0.15f);
                }
            }
            
            // Random struggle force based on distance from player and current tension
            float distance = Vector2.Distance(hookedFish.transform.position, player.transform.position);
            float distanceMultiplier = Mathf.Clamp(distance / 4f, 0.8f, 1.5f);
            
            // Fish struggles harder when close to being caught
            if (distance < captureDistance * 1.5f)
            {
                struggleStrength *= 1.5f; // Last-ditch effort to escape
            }
            
            // Fish struggles differently based on current tension
            float tensionFactor = 1.0f;
            if (currentTension < sweetSpotMin) // Low tension
            {
                tensionFactor = 0.8f; // Less struggle when tension is low
            }
            else if (currentTension > sweetSpotMax) // High tension
            {
                tensionFactor = 1.3f; // More struggle when tension is high
            }
            
            // Apply a strong impulse force in a direction away from player
            float randomForce = struggleStrength * UnityEngine.Random.Range(0.8f, 1.3f) * distanceMultiplier * tensionFactor;
            fishRb.AddForce(awayFromPlayer * randomForce, ForceMode2D.Impulse);
            
            // Increase tension during struggles
            currentTension += randomForce * 0.15f;
            
            // Set next struggle time with size-based variability
            float intervalMultiplier = 1.0f;
            if (fishAI != null && fishAI.fishData != null && fishAI.fishData.sizeMultiplier > 0)
            {
                // Smaller fish struggle more frequently
                intervalMultiplier = Mathf.Lerp(0.8f, 1.2f, Mathf.Clamp01(fishAI.fishData.sizeMultiplier));
            }
            
            nextStruggleTime = Time.time + UnityEngine.Random.Range(
                fishStruggleInterval * intervalMultiplier / struggleVariability, 
                fishStruggleInterval * intervalMultiplier * struggleVariability
            );
        }

        // Natural tension decrease over time (handled in HandlePlayerInput)
    }

    private void HandlePlayerInput()
    {
        // When space is pressed, pull the fish toward the player
        if (Input.GetKey(KeyCode.Space))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null || fishRb == null) return;

            // Calculate pull force with sweet spot and distance factors
            Vector2 towardPlayer = (player.transform.position - hookedFish.transform.position).normalized;
            float distance = Vector2.Distance(hookedFish.transform.position, player.transform.position);
            
            // Base pull strength depends on distance - need more power when far
            float distanceBonus = Mathf.Clamp(distance / 6f, 0.8f, 1.5f);
            
            // Variable pull multiplier based on tension "sweet spot"
            float pullMultiplier = 1.0f;
            
            // Check if we're in the optimal tension range (sweet spot)
            if (currentTension >= sweetSpotMin && currentTension <= sweetSpotMax)
            {
                // Strong bonus when in sweet spot
                pullMultiplier = sweetSpotBonus;
                bonusPullTimer += Time.deltaTime;
                
                // Visual feedback would be in UpdateTensionBar method
            }
            else
            {
                // Gradually lose sweet spot time when outside optimal range
                bonusPullTimer = Mathf.Max(0, bonusPullTimer - Time.deltaTime * 0.5f);
                
                // Pulling is less effective when tension is too high
                if (currentTension > sweetSpotMax)
                {
                    // The higher the tension, the less effective pulling becomes
                    float highTensionPenalty = Mathf.Lerp(1.0f, 0.5f, (currentTension - sweetSpotMax) / (maxTension - sweetSpotMax));
                    pullMultiplier *= highTensionPenalty;
                }
            }
            
            // Apply force toward player with all factors combined
            fishRb.AddForce(towardPlayer * playerForce * pullMultiplier * distanceBonus * Time.deltaTime, ForceMode2D.Impulse);

            // Tension increases faster when fish is struggling and based on fish size/weight
            float tensionBuildupRate = tensionIncreaseRate;
            
            // Fish size affects tension buildup - bigger fish create more tension
            FishAI fishAI = hookedFish.GetComponent<FishAI>();
            if (fishAI != null && fishAI.fishData != null && fishAI.fishData.sizeMultiplier > 0)
            {
                // Larger fish create more tension
                tensionBuildupRate *= Mathf.Lerp(0.8f, 1.5f, Mathf.Clamp01(fishAI.fishData.sizeMultiplier - 0.4f) / 0.6f);
            }
            
            // Reduce tension buildup when fish is close to player (within about 3 units)
            // This makes it easier to finish landing the fish once it's close
            float distanceToPlayer = Vector2.Distance(hookedFish.transform.position, player.transform.position);
            float closenessFactor = Mathf.Clamp01(distanceToPlayer / 4f);  // 0 when very close, 1 when far
            tensionBuildupRate *= 0.5f + (closenessFactor * 0.5f);  // Reduced by up to 50% when very close
            
            // Apply tension increase with all factors
            currentTension += tensionBuildupRate * Time.deltaTime;
            
            // Track last reel time for tension decay pause
            lastReelTime = Time.time;
        }
        else
        {
            // When not reeling, tension decreases after the pause
            if (Time.time > lastReelTime + tensionDecayPause)
            {
                // Faster decay when very high tension
                float decayMultiplier = (currentTension > maxTension * 0.7f) ? 1.5f : 1.0f;
                currentTension -= tensionDecreaseRate * decayMultiplier * Time.deltaTime;
            }
        }
        
        // Quick tension release with Down Arrow or S key - with cooldown and penalty for rapid use
        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)))
        {
            float currentTime = Time.time;
            float timeSinceLastDownshift = currentTime - lastDownshiftTime;
            
            // Check if downshift is happening too quickly (spamming)
            if (timeSinceLastDownshift < downshiftCooldown)
            {
                // Rapid downshift detected! Add tension instead of releasing it
                consecutiveDownshifts++;
                
                // Penalty increases with consecutive rapid downshifts
                float scaledPenalty = rapidDownshiftPenalty * (1.0f + (consecutiveDownshifts * 0.3f));
                
                // Apply the penalty - adds tension instead of relieving it
                currentTension += scaledPenalty;
                
                // Visual feedback - flash the tension bar red
                if (tensionBarMask != null)
                {
                    // Store current time to reset the color after a delay
                    colorResetTime = Time.time + 0.2f;
                    // Flash red to indicate penalty
                    tensionBarMask.color = Color.red;
                }
                
                // Audio feedback for tension spike (if hooked fish has an audio source)
                if (hookedFish != null)
                {
                    AudioSource audioSource = hookedFish.GetComponent<AudioSource>();
                    if (audioSource != null)
                    {
                        // Play an error sound at a higher pitch when spamming downshift
                        audioSource.pitch = 1.0f + (consecutiveDownshifts * 0.1f);
                        audioSource.Play();
                    }
                }
                
                // Haptic feedback - make the lure move erratically
                if (fishRb != null)
                {
                    // Add random force in all directions to simulate line tension spike
                    Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
                    fishRb.AddForce(randomDirection * (scaledPenalty * 0.5f), ForceMode2D.Impulse);
                    
                    // Find player to calculate fish escape direction
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null)
                    {
                        // Calculate direction away from player
                        Vector2 awayFromPlayer = (hookedFish.transform.position - player.transform.position).normalized;
                        
                        // Rapid downshifts trigger fish struggle - they feel the line weakness
                        // Stronger escape attempt when sensing line weakness from rapid downshifts
                        float escapePower = Mathf.Min(consecutiveDownshifts * 15f, 60f);
                        fishRb.AddForce(awayFromPlayer * escapePower, ForceMode2D.Impulse);
                    }
                }
            }
            else
            {
                // Normal downshift with sufficient cooldown - significantly more effective
                currentTension -= 30f; // Further increased from 25f for easier fishing
                // Reset consecutive counter when doing proper downshifts
                consecutiveDownshifts = 0;
            }
            
            // Update the last downshift time regardless
            lastDownshiftTime = currentTime;
            
            // Activate visual cooldown indicator
            cooldownVisualActive = true;
        }
    }

    private void UpdateTensionBar()
    {
        // Clamp tension value
        currentTension = Mathf.Clamp(currentTension, 0, maxTension);

        // Update UI if available
        if (tensionBarMask != null)
        {
            float fillAmount = currentTension / maxTension;     
            tensionBarMask.fillAmount = fillAmount;
            
            // Check if we need to reset the flash color (for rapid downshift penalty visual)
            if (colorResetTime > 0 && Time.time >= colorResetTime)
            {
                // Reset the color flag
                colorResetTime = -1f;
                // Color will be set below based on current tension
            }
            
            // Check downshift cooldown status for visual indicator
            float timeSinceLastDownshift = Time.time - lastDownshiftTime;
            
            // Create "ready" visual indicator when cooldown is complete
            if (cooldownVisualActive && timeSinceLastDownshift >= downshiftCooldown)
            {
                // Pulse the tension bar border to indicate ready status
                // Find the border element (should be a child of the tension bar)
                Transform tensionBarBorder = tensionBarGameObject.transform.Find("Border");
                if (tensionBarBorder != null)
                {
                    Image borderImage = tensionBarBorder.GetComponent<Image>();
                    if (borderImage != null)
                    {
                        // Flash the border to indicate ready status
                        borderImage.color = Color.white; // Set to bright white when ready
                    }
                }
                
                // Reset the cooldown visual flag
                cooldownVisualActive = false;
            }
            else if (cooldownVisualActive)
            {
                // Show cooldown progress on border
                Transform tensionBarBorder = tensionBarGameObject.transform.Find("Border");
                if (tensionBarBorder != null)
                {
                    Image borderImage = tensionBarBorder.GetComponent<Image>();
                    if (borderImage != null)
                    {
                        // Calculate cooldown progress (0 to 1)
                        float cooldownProgress = timeSinceLastDownshift / downshiftCooldown;
                        
                        // Darker border during cooldown that gradually brightens
                        float alpha = Mathf.Lerp(0.2f, 0.7f, cooldownProgress);
                        borderImage.color = new Color(0.5f, 0.5f, 0.5f, alpha);
                    }
                }
                
                // Draw an arrow indicator at bottom of screen that shows cooldown status
                // This is a separate UI element that we'll create in OnGUI
            }
            
            // Only update color if not in penalty flash state
            if (colorResetTime <= 0)
            {
                // Color-coded sweet spot system with visual feedback
                if (currentTension >= sweetSpotMin && currentTension <= sweetSpotMax)
                {
                    // Sweet spot - bright green with subtle pulsing for positive feedback
                    float sweetPulse = Mathf.PingPong(Time.time * 3f, 0.2f);
                    tensionBarMask.color = new Color(0.2f, 0.9f + sweetPulse * 0.1f, 0.2f); // Pulsing green
                }
                else if (currentTension < sweetSpotMin)
                {
                    // Too low tension - blue, need to reel more
                    float blueDarkness = Mathf.Lerp(0.4f, 0.9f, currentTension / sweetSpotMin);
                    tensionBarMask.color = new Color(0.2f, 0.7f, blueDarkness); // Blue (darker when farther from sweet spot)
                }
                else if (currentTension < maxTension * 0.75f)
                {
                    // Above sweet spot but not dangerous - amber/yellow
                    tensionBarMask.color = new Color(0.9f, 0.8f, 0.2f); // Yellow
                }
                else if (currentTension < maxTension * 0.9f)
                {
                    // High tension - orange warning
                    tensionBarMask.color = new Color(0.9f, 0.5f, 0.1f); // Orange
                }
                else
                {
                    // Critical tension - red with strong pulsing effect
                    tensionBarMask.color = new Color(0.9f, 0.2f, 0.2f); // Red
                    
                    // Dramatic pulsing effect for danger
                    float pulse = Mathf.PingPong(Time.time * 8f, 0.6f);
                    tensionBarMask.color = new Color(0.9f + pulse * 0.1f, 0.1f, 0.1f);
                }
            }
            
            // Show sweet spot markers on the bar
            // This would ideally be implemented with additional UI elements marking the sweet spot range
        }
    }
    
    private void CheckFishStatus()
    {
        // Check if tension too high - fish breaks the line
        if (currentTension >= maxTension)
        {
            // Fish escapes - reset and go back to passive state
            GameObject stateControllerObj = GameObject.FindWithTag("GameController");
            if (stateControllerObj != null)
            {
                StateController controller = stateControllerObj.GetComponent<StateController>();
                if (controller != null)
                {                 
                    controller.FishEscaped();
                    return;
                }
            }
        }

        // Check if fish is close enough to player to be caught
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && hookedFish != null)
        {
            float distanceToPlayer = Vector2.Distance(player.transform.position, hookedFish.transform.position);
            
            // Fish data for catch calculations
            FishAI fishAI = hookedFish.GetComponent<FishAI>();
            float rarityModifier = 1.0f;
            float sizeModifier = 1.0f;
            
            // Larger/rarer fish are harder to catch
            if (fishAI != null && fishAI.fishData != null)
            {
                // Size affects catch difficulty
                if (fishAI.fishData.sizeMultiplier > 0)
                {
                    // Bigger fish need to be closer to catch
                    sizeModifier = Mathf.Lerp(1.2f, 0.8f, Mathf.Clamp01((fishAI.fishData.sizeMultiplier - 0.4f) / 0.6f));
                }
                
                // Rarity affects catch difficulty
                if (fishAI.fishData.rarity > Rarity.Common)
                {
                    // Rarer fish need more sweet spot time
                    rarityModifier = 1.0f - ((float)fishAI.fishData.rarity * 0.1f);
                }
            }
            
            // Sweet spot time bonus - more time spent in sweet spot makes catching easier
            float sweetSpotEfficiency = Mathf.Clamp01(bonusPullTimer / (3.0f + (float)Mathf.Max(0, (int)fishAI.fishData.rarity)));
            float timeBonus = Mathf.Lerp(0.0f, 1.5f, sweetSpotEfficiency); 
            
            // Calculate final capture distance based on all factors
            float effectiveCaptureDistance = captureDistance * (1f + timeBonus) * sizeModifier * rarityModifier;
            
            // Slightly easier to catch when fish are in schools (distracted)
            Collider2D[] nearbyFish = Physics2D.OverlapCircleAll(hookedFish.transform.position, 2.0f);
            if (nearbyFish.Length > 2) // If there are fish nearby
            {
                // Small bonus for school distraction
                effectiveCaptureDistance *= 1.1f; 
            }
            
            // Check if close enough to catch
            if (distanceToPlayer < effectiveCaptureDistance)
            {
                // Return to passive state with caught fish
                GameObject stateControllerObj = GameObject.FindWithTag("GameController");
                if (stateControllerObj != null)
                {
                    StateController controller = stateControllerObj.GetComponent<StateController>();
                    if (controller != null)
                    {
                        controller.FishCaught(hookedFish);
                    }
                }
            }
        }
    }
    
    // Variable to track when we should reset the tension bar color
    private float colorResetTime = -1f;
    
    // Add helper text to the screen for instructions
    private void OnGUI()
    {
        GUI.Label(new Rect(20, Screen.height - 120, 500, 30), "Hold SPACE to reel in. Release to rest line.");
        GUI.Label(new Rect(20, Screen.height - 100, 500, 30), "Keep tension in the GREEN sweet spot for bonus strength!");
        GUI.Label(new Rect(20, Screen.height - 80, 500, 30), "Time spent in sweet spot makes catching easier.");
        GUI.Label(new Rect(20, Screen.height - 60, 500, 30), "Press DOWN ARROW to release tension in emergencies.");
        GUI.Label(new Rect(20, Screen.height - 40, 500, 30), "WARNING: Rapid downshifts will INCREASE tension instead!");
        
        // Display cooldown status
        float timeSinceLastDownshift = Time.time - lastDownshiftTime;
        float cooldownProgress = Mathf.Clamp01(timeSinceLastDownshift / downshiftCooldown);
        
        if (cooldownProgress < 1.0f)
        {
            // Create a cooldown text indicator
            string cooldownText = "COOLDOWN: ";
            int barLength = 10;
            int filledSegments = Mathf.RoundToInt(cooldownProgress * barLength);
            
            // Create a progress bar using ASCII characters
            for (int i = 0; i < barLength; i++)
            {
                cooldownText += (i < filledSegments) ? "█" : "▒";
            }
            
            // Add percentage
            cooldownText += string.Format(" {0}%", Mathf.RoundToInt(cooldownProgress * 100));
            
            // Choose color: red -> yellow -> green as cooldown completes
            if (cooldownProgress < 0.5f)
                GUI.color = Color.red;
            else if (cooldownProgress < 0.9f)
                GUI.color = Color.yellow;
            else
                GUI.color = Color.green;
                
            // Draw centered at bottom
            float textWidth = 300;
            GUI.Label(new Rect((Screen.width - textWidth) / 2, Screen.height - 20, textWidth, 30), cooldownText);
            
            // Reset color
            GUI.color = Color.white;
        }
        else if (cooldownVisualActive == false)
        {
            // Show "READY!" indicator when cooldown is complete
            GUI.color = Color.green;
            float textWidth = 300;
            GUI.Label(new Rect((Screen.width - textWidth) / 2, Screen.height - 20, textWidth, 30), "↓ TENSION RELEASE READY! ↓");
            GUI.color = Color.white;
        }
    }
}