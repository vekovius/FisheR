using UnityEngine;

public class InWaterState : StateInterface
{
    private GameObject lure;
    private float moveSpeed = 1.8f;  // Increased speed for better control and faster sinking
    private float waterLevel;
    private float maxSpeed = 4f;     // Increased maximum speed in water for faster sinking
    private float dampingFactor = 0.95f; // Reduced damping for less water resistance and faster sinking
    
    // Lure movement constraints
    private float rightwardMovementFactor = 0.1f;  // Very strong restriction on rightward movement after cast
    private Transform playerTransform;      // Reference to the player for position comparison
    private float initialXPosition;         // X position when lure first enters water
    private bool xPositionInitialized = false; // Flag to track if initial position is set

    public void Enter()
    {
        lure = GameObject.FindWithTag("Lure");
        
        // Find the water level from StateController
        StateController stateController = GameObject.FindObjectOfType<StateController>();
        if (stateController != null)
        {
            waterLevel = stateController.waterLevel;
        }
        
        // Find the player for position reference
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found for lure movement calculations");
        }
        
        // Reset initialization flag - we'll capture the lure's initial X position in the first Update
        xPositionInitialized = false;
        
        // Apply realistic water physics to lure
        if (lure != null)
        {
            Rigidbody2D rb = lure.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Significant velocity reduction on water impact
                rb.linearVelocity *= 0.2f;
                
                // Much more realistic gravity - lures sink significantly faster in water
                rb.gravityScale = 0.8f; // Greatly increased gravity to make lure sink much faster
                
                // Strong downward force to simulate heavy lure sinking rapidly
                rb.AddForce(Vector2.down * 5.0f, ForceMode2D.Force);
                
                // Lower drag to allow faster sinking
                rb.linearDamping = 1.2f; // Reduced drag so lure sinks much more naturally
                rb.angularDamping = 2.0f;
                
                // Apply slight random movement to simulate water currents affecting lure
                rb.AddTorque(UnityEngine.Random.Range(-0.1f, 0.1f), ForceMode2D.Impulse);
            }
        }
    }

    public void Update()
    {
        if (lure == null) return;
        
        // Keep the lure below water level
        if (lure.transform.position.y > waterLevel)
        {
            Vector3 position = lure.transform.position;
            position.y = waterLevel;
            lure.transform.position = position;
        }
        
        HandleLureMovement();
    }

    public void Exit()
    {      
    }

    private void HandleLureMovement()
    {
        Rigidbody2D rb = lure.GetComponent<Rigidbody2D>();
        if (rb == null) return;
        
        // Capture initial X position when lure first enters water
        if (!xPositionInitialized && lure != null)
        {
            initialXPosition = lure.transform.position.x;
            xPositionInitialized = true;
            Debug.Log("Lure initial X position set to: " + initialXPosition);
        }
        
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        // Apply modified movement based on direction
        Vector2 movementDirection = new Vector2(horizontalMovement, verticalMovement);
        
        if (movementDirection.magnitude > 0.1f)
        {
            // Check if trying to move rightward (further from cast point)
            if (horizontalMovement > 0 && lure.transform.position.x >= initialXPosition)
            {
                // Greatly restrict rightward movement beyond the initial landing point
                // This simulates a real fishing line that can't be directed further after the cast
                horizontalMovement *= rightwardMovementFactor; // Apply very strong restriction
            }
            
            // Apply different movement based on direction
            movementDirection = new Vector2(horizontalMovement, verticalMovement);
            movementDirection.Normalize();
            rb.AddForce(movementDirection * moveSpeed);
        }
        
        // Apply water resistance
        rb.linearVelocity *= dampingFactor;
        
        // Clamp velocity to prevent excessive speed
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
        
        // Add subtle random movement to simulate water currents
        if (Random.value < 0.1f)
        {
            Vector2 randomForce = Random.insideUnitCircle * 0.05f;
            rb.AddForce(randomForce, ForceMode2D.Force);
        }
        
        // Add constant downward force to ensure lure always sinks naturally
        // This ensures the lure continues to sink even when being moved
        rb.AddForce(Vector2.down * 1.5f, ForceMode2D.Force);
        
        // Visual debug of boundary (draw a vertical line at the initial X position)
        if (xPositionInitialized)
        {
            Debug.DrawLine(
                new Vector3(initialXPosition, waterLevel + 5f, 0), 
                new Vector3(initialXPosition, waterLevel - 10f, 0), 
                Color.yellow);
        }
    }

    public bool IsFishHooked()
    {
        Collider2D lureCollider = lure.GetComponent<Collider2D>();
        if (lureCollider == null)
        {
            Debug.LogError("Failed to get lure collider");
            return false;
        }

        Collider2D[] overlappingColliders = Physics2D.OverlapCircleAll(lure.transform.position, lureCollider.bounds.extents.magnitude);

        foreach (Collider2D collider in overlappingColliders)
        {
            FishAI fish = collider.GetComponent<FishAI>();
            if (fish!= null)
            {     
                return true;
            }
        }

        return false;
    }
}
