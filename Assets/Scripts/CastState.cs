using UnityEngine;

public class CastState : StateInterface
{
    private float castSpeed;
    private float maxCastSpeed;
    private readonly GameObject lurePrefab;
    private readonly Transform castOrigin;
    private readonly DirectionIndicator directionIndicator;
    private GameObject currentLure;
    private readonly IPowerMinigame powerMinigame;
    private float currentPowerBarValue;


    public CastState(
        float castSpeed,
        float maxCastSpeed,
        GameObject lurePrefab,
        Transform castOrigin,
        DirectionIndicator directionIndicator,
        IPowerMinigame powerMinigame)
    {
        this.castSpeed = castSpeed;
        this.maxCastSpeed = maxCastSpeed;
        this.lurePrefab = lurePrefab;
        this.castOrigin = castOrigin;
        this.directionIndicator = directionIndicator;
        this.powerMinigame = powerMinigame;
        this.currentPowerBarValue = 0;
    }

    public void Enter()
    {
        // Subscribe to the power changed event
        powerMinigame.OnPowerChanged += UpdatePowerValue;
        powerMinigame.Activate();
    }

    public void UpdatePowerValue(float powerValue)
    {
        currentPowerBarValue = powerValue;
    }

    public void Update()
    {
        //No logic needed here for now.
    }

    public void Exit()
    {
        //Hide the direction indicator when exiting cast state
        if (directionIndicator != null && directionIndicator.gameObject != null)
        {
            directionIndicator.gameObject.SetActive(false);
        }


        //Unsubscribe to prevent memeory leak
        powerMinigame.OnPowerChanged -= UpdatePowerValue;
        powerMinigame.Deactivate();

        Vector2 castDirection = directionIndicator.getCurrentDirection();
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        CastLure(castDirection);
        cameraController.target = currentLure.transform;  
    }

    private void CastLure(Vector2 castDirection)
    {
        currentLure = Object.Instantiate(lurePrefab, castOrigin.position, Quaternion.identity);
        Rigidbody2D rb = currentLure.GetComponent<Rigidbody2D>();

        // Set normal gravity for in-air physics
        rb.gravityScale = 1.0f;

        //Use the tracked power value 
        float appliedPowerValue = currentPowerBarValue;
        castSpeed = maxCastSpeed * appliedPowerValue;
        
        rb.AddForce(castDirection * castSpeed, ForceMode2D.Impulse);
    }
}
