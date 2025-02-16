using UnityEngine;

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
    public float castSpeed = 10f;
    public GameObject lurePrefab;
    public Transform castOrigin;
    public DirectionIndicator directionIndicator;
    public KeyCode castKey = KeyCode.Space;

    private PassiveState passiveState;
    private CastState castState;
    private InAirState inAirState;
    public CastingMinigame castingMinigame;
    public CameraController cameraController;


    private void Start()
    {
        passiveState = new PassiveState(inventoryPanel, mapPanel, settingsPanel, inventoryKey, mapKey, settingsKey);
        castState = new CastState(castSpeed, lurePrefab, castOrigin, directionIndicator);
        inAirState = new InAirState();

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

        if (currentState is PassiveState && Input.GetKeyDown(castKey))
        {
            Debug.Log("Chaning to cast state");
            ChangeState(castState);
        }

        if (currentState is CastState && Input.GetKeyUp(castKey))
        {
            Debug.Log("Chaning to inAirState state");
            directionIndicator.gameObject.SetActive(false);
            ChangeState(inAirState);
        }
    }

}
