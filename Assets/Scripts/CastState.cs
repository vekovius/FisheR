using UnityEngine;

public class CastState : StateInterface
{
    private readonly float castSpeed;
    private readonly GameObject lurePrefab;
    private readonly Transform castOrigin;
    private readonly DirectionIndicator directionIndicator;
    private GameObject currentLure;

    public CastState(
        float castSpeed,
        GameObject lurePrefab,
        Transform castOrigin,
        DirectionIndicator directionIndicator)
    {
        this.castSpeed = castSpeed;
        this.lurePrefab = lurePrefab;
        this.castOrigin = castOrigin;
        this.directionIndicator = directionIndicator;
    }

    public void Enter()
    {
        // Initialize state if needed
    }

    public void Update()
    {

    }

    public void Exit()
    {
        Vector2 castDirection = directionIndicator.getCurrentDirection();
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        CastLure(castDirection);
        cameraController.target = currentLure.transform;
        
        
    }

    private void CastLure(Vector2 castDirection)
    {
        currentLure = Object.Instantiate(lurePrefab, castOrigin.position, Quaternion.identity);
        Rigidbody2D rb = currentLure.GetComponent<Rigidbody2D>();
        rb.AddForce(castDirection * castSpeed, ForceMode2D.Impulse);
    }
}
