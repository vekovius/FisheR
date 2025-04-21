using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject mapPanel;

    void Awake()
    {
        DontDestroyOnLoad(this);

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Toggles all panels off
    public void TogglePanelsOff()
    {
        settingsPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        mapPanel.SetActive(false);
    }
}
