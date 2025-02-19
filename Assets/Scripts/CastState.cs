using UnityEngine;

public class CastState : StateInterface
{
    private float castSpeed;
    private float maxCastSpeed;
    private readonly GameObject lurePrefab;
    private readonly Transform castOrigin;
    private readonly DirectionIndicator directionIndicator;
    private GameObject currentLure;
    private CastingMinigame castingMinigame;
    

    public CastState(
        float castSpeed,
        float maxCastSpeed,
        GameObject lurePrefab,
        Transform castOrigin,
        DirectionIndicator directionIndicator,
        CastingMinigame castingMinigame)
    {
        this.castSpeed = castSpeed;
        this.maxCastSpeed = maxCastSpeed;
        this.lurePrefab = lurePrefab;
        this.castOrigin = castOrigin;
        this.directionIndicator = directionIndicator;
        this.castingMinigame = castingMinigame;
    }

    public void Enter()
    {
        castingMinigame.enabled = true;
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
        castSpeed = maxCastSpeed * (castingMinigame.getCurrentPowerBarValue()/100);
        //Debug.Log($"Casting with speed {castSpeed}");
        rb.AddForce(castDirection * castSpeed, ForceMode2D.Impulse);
    }
}
