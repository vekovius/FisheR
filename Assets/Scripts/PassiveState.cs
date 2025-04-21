using UnityEngine;
using UnityEngine.UI;

public class PassiveState : StateInterface
{
    private GameObject inventoryPanel;
    private GameObject mapPanel;
    private GameObject settingsPanel;
    private GameObject directionIndicator;

    public KeyCode inventoryKey;
    public KeyCode mapKey;
    public KeyCode settingsKey;

    public PassiveState(
        GameObject inventoryPanel,
        GameObject mapPanel,
        GameObject settingsPanel,
        KeyCode inventoryKey,
        KeyCode mapKey,
        KeyCode settingsKey,
        GameObject directionIndicator = null
        )
    {
        this.inventoryPanel = inventoryPanel;
        this.mapPanel = mapPanel;
        this.settingsPanel = settingsPanel;
        this.inventoryKey = inventoryKey;
        this.mapKey = mapKey;
        this.settingsKey = settingsKey;
        this.directionIndicator = directionIndicator;
    }

    public void Enter()
    {
        inventoryPanel.SetActive(false);
        mapPanel.SetActive(false);
        settingsPanel.SetActive(false);

        //Change camera to track player
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        cameraController.target = playerObject.transform;

        if (directionIndicator != null)
        {
            directionIndicator.SetActive(true);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(inventoryKey))
        {
            TogglePanel(inventoryPanel);
        }
        if (Input.GetKeyDown(mapKey))
        {
            TogglePanel(mapPanel);
        }
        if (Input.GetKeyDown(settingsKey))
        {
            TogglePanel(settingsPanel);
        }
    }

    public void Exit()
    {
    }

    void TogglePanel(GameObject panel)
    {
        if (panel == null)
        {
            Debug.LogError("Panel is not assigned in the TogglePanel method.");
            GameObject.FindGameObjectWithTag(panel.name).SetActive(false);
            return;
        }
        //Simple toggle, activeSelf returns current state
        //Then SetActive will set it to negation of the return
        panel.SetActive(!panel.activeSelf);
    }
}
