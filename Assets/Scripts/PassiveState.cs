using UnityEngine;
using UnityEngine.UI;

public class PassiveState : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject inventoryPanel;
    public GameObject mapPanel;
    public GameObject settingsPanel;

    [Header("KeyBindings")]
    public KeyCode inventoryKey = KeyCode.I;
    public KeyCode mapKey = KeyCode.M;
    public KeyCode settingsKey = KeyCode.Escape;

    private void Update()
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

    void TogglePanel(GameObject panel)
    {
       //Simple toggle, activeSelf returns current state
       //Then SetActive will set it to negation of the return
        panel.SetActive(!panel.activeSelf);
    }
}
