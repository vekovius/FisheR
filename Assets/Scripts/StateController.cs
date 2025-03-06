using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StateController : MonoBehaviour
{
    private StateInterface currentState;

    [Header("Passive State Settings")]
    public GameObject inventoryPanel;
    public GameObject mapPanel;
    public GameObject settingsPanel;
    public KeyCode inventoryKey = KeyCode.I;
    public KeyCode mapKey = KeyCode.M;
    public KeyCode settingsKey = KeyCode.Escape;

    [Header("Cast State Settings")]
    public float castSpeed;
    public float maxCastSpeed;
    public GameObject lurePrefab;
    public Transform castOrigin;
    public DirectionIndicator directionIndicator;
    public KeyCode castKey = KeyCode.Space;


    [Header("Water settings")]
    public float waterLevel = 16f;


    private PassiveState passiveState;
    private CastState castState;
    private InAirState inAirState;
    private InWaterState inWaterState;
    private HookedState hookedState;

    public CastingMinigame castingMinigame;
    public CameraController cameraController;


    private void Start()
    {
        passiveState = new PassiveState(inventoryPanel, mapPanel, settingsPanel, inventoryKey, mapKey, settingsKey);
        castState = new CastState(castSpeed, maxCastSpeed, lurePrefab, castOrigin, directionIndicator, castingMinigame);
        inAirState = new InAirState(waterLevel);
        inWaterState = new InWaterState();
        hookedState = new HookedState();

        ChangeState(passiveState);
    }
    public void ChangeState(StateInterface newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;

        if (currentState is CastState)
        {
            castingMinigame.gameObject.SetActive(true);
        }
        else
        {
            castingMinigame.gameObject.SetActive(false);
        }


        if (currentState != null)
        {
            currentState.Enter();
        }

    }

    private void Update()
    {
        if (currentState != null)
            currentState.Update();

        HandleStateTransitions();

        HandleKeyInputs();
    }

    private void HandleStateTransitions()
    {
        if (currentState is InAirState)
        {
            InAirState airState = (InAirState)currentState;
            if (airState.IsLureInWater())
            {
                ChangeState(inWaterState);
            }
        }

        if (currentState is InWaterState)
        {
            InWaterState waterState = (InWaterState)currentState;
            if (waterState.IsFishHooked())
            {
                Debug.Log("Transitioning to IsHookedState");
                ChangeState(hookedState);
            }
        }
    }

    private void HandleKeyInputs()
    {
        if (currentState is PassiveState && Input.GetKeyDown(castKey))
        {
            ChangeState(castState);
        }
        
        if (currentState is CastState && Input.GetKeyUp(castKey))
        {
            directionIndicator.gameObject.SetActive(false);
            ChangeState(inAirState);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            directionIndicator.gameObject.SetActive(true);
            GameObject Lure = GameObject.FindGameObjectWithTag("Lure");
            Object.Destroy(Lure);
            ChangeState(passiveState);
        }
    }

}
