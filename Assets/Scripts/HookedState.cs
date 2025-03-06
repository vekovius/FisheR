using UnityEngine;
using UnityEngine.UI;

public class HookedState : StateInterface
{
    private GameObject lure;
    private GameObject hookedFish;
    private Rigidbody2D fishRb;
    private float struggleForce = 5f;
    private float playerForce = 8f;

    // Reference to tension bar UI elements
    private GameObject tensionBarGameObject;
    private Image tensionBarMask;
    private float currentTension;
    private float maxTension = 100f;
    private float tensionDecreaseRate = 5f;
    private float tensionIncreaseRate = 20f;

    public HookedState(GameObject tensionBarGameObject = null)
    {
        this.tensionBarGameObject = tensionBarGameObject;
    }

    public void Enter()
    {
        Debug.Log("Entered hooked state");

        // Find the lure in the scene
        lure = GameObject.FindWithTag("Lure");
        if (lure == null)
        {
            Debug.LogError("No lure found in HookedState");
            return;
        }

        // Find the hooked fish by checking for fish near the lure
        FindHookedFish();

        // Setup tension bar if available
        SetupTensionBar();

        // Initialize tension value
        currentTension = maxTension / 2;

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
        Debug.Log("Exited hooked state");

        // Re-enable fish AI if not caught
        if (hookedFish != null)
        {
            FishAI fishAI = hookedFish.GetComponent<FishAI>();
            if (fishAI != null)
            {
                fishAI.enabled = true;
            }

            // Remove the joint connecting fish and lure
            FixedJoint2D joint = hookedFish.GetComponent<FixedJoint2D>();
            if (joint != null)
            {
                Destroy(joint);
            }
        }

        // Mark the lure as free again if it exists
        if (lure != null)
        {
            // Re-enable lure collider
            Collider2D lureCollider = lure.GetComponent<Collider2D>();
            if (lureCollider != null)
            {
                lureCollider.enabled = true;
            }

            // If lure was parented to the fish, restore its original parent
            if (lure.transform.parent == hookedFish?.transform)
            {
                lure.transform.SetParent(null);
            }

            LureStateController lureState = lure.GetComponent<LureStateController>();
            if (lureState != null)
            {
                lureState.SetFree();
            }
        }

        // Hide tension bar
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
                Debug.Log($"Hooked fish: {hookedFish.name}");

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

        // Calculate hook position - usually the mouth of the fish
        // For simplicity, we'll use a fixed offset from the center of the fish
        // In a real game, you might want to define a specific "mouth" position on your fish model
        Vector2 hookPosition = hookedFish.transform.position;

        // You could adjust this based on the fish's forward direction
        Vector2 fishForward = hookedFish.transform.right; // Assuming fish faces right by default
        hookPosition += fishForward * 0.5f; // Offset to the front of the fish

        // Physically attach lure to fish
        // Option 1: Using a FixedJoint2D (physically accurate, allows some flexibility)
        FixedJoint2D joint = hookedFish.AddComponent<FixedJoint2D>();
        joint.connectedBody = lure.GetComponent<Rigidbody2D>();
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = Vector2.zero; // Center of the fish

        // Option 2: If you prefer the lure to be exactly at a specific position on the fish
        // Make the lure a child of the fish - uncomment this if you prefer this approach
        // lure.transform.SetParent(hookedFish.transform);
        // lure.transform.localPosition = new Vector3(0.5f, 0, 0); // Adjust based on your fish model

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
            tensionBarMask = tensionBarGameObject.GetComponentInChildren<Image>();

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

        // Rotate fish to face away from player (add 180 degrees to face away)
        float angle = Mathf.Atan2(awayFromPlayer.y, awayFromPlayer.x) * Mathf.Rad2Deg;
        hookedFish.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Apply force to fish to swim away from player
        fishRb.AddForce(awayFromPlayer * struggleForce * Time.deltaTime, ForceMode2D.Impulse);

        // Decrease tension as fish struggles
        currentTension -= tensionDecreaseRate * Time.deltaTime;
    }

    private void HandlePlayerInput()
    {
        // When space is pressed, pull the fish toward the player
        if (Input.GetKey(KeyCode.Space))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null || fishRb == null) return;

            Vector2 towardPlayer = (player.transform.position - hookedFish.transform.position).normalized;
            fishRb.AddForce(towardPlayer * playerForce * Time.deltaTime, ForceMode2D.Impulse);

            // Increase tension when reeling
            currentTension += tensionIncreaseRate * Time.deltaTime;
        }
    }

    private void UpdateTensionBar()
    {
        // Clamp tension value
        currentTension = Mathf.Clamp(currentTension, 0, maxTension);

        // Update UI if available
        if (tensionBarMask != null)
        {
            tensionBarMask.fillAmount = currentTension / maxTension;
        }
    }

    private void CheckFishStatus()
    {
        // Check if tension is too low (fish escapes)
        if (currentTension <= 0)
        {
            Debug.Log("Fish escaped - tension too low!");
            // Fish escapes - reset and go back to passive state
            GameObject stateControllerObj = GameObject.FindWithTag("GameController");
            if (stateControllerObj != null)
            {
                StateController controller = stateControllerObj.GetComponent<StateController>();
                if (controller != null)
                {
                    controller.FishEscaped();
                }
            }
        }

        // Check if fish is close enough to player to be caught
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && hookedFish != null)
        {
            float distanceToPlayer = Vector2.Distance(player.transform.position, hookedFish.transform.position);
            if (distanceToPlayer < 2f)
            {
                Debug.Log("Fish caught successfully!");
                // Trigger the event for fish being caught
                /*
                if (StateController.OnFishCaught != null)
                {
                    StateController.OnFishCaught(hookedFish);
                }
                */

                // Return to passive state
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
}