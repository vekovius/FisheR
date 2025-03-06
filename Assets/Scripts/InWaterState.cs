using UnityEngine;

public class InWaterState : StateInterface
{
    private GameObject lure;
    private float moveSpeed = 2f;

    public void Enter()
    {
        lure = GameObject.FindWithTag("Lure");
        Debug.Log("Entering inWaterState");

    }

    public void Update()
    {
        if (lure == null) return;
        HandleLureMovement();
    }

    public void Exit()
    {
        Debug.Log("Exit InWaterState");
    }

    private void HandleLureMovement()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        Vector2 movementDirection = new Vector2(horizontalMovement, verticalMovement).normalized;

        Rigidbody2D rb = lure.GetComponent<Rigidbody2D>();
        rb.AddForce(movementDirection * moveSpeed);
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
                Debug.Log("Fish Hooked");

                return true;
            }
        }

        return false;
    }
}
