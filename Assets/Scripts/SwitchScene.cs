using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public bool fromMap = false;

    public void Switch(string name) 
    {
        if (fromMap) 
        {
            GameObject.FindGameObjectWithTag("MapPanel").SetActive(false);
        }
        LevelManager.Instance.LoadScene(name);
        UIManager.Instance.TogglePanelsOff();
    }
    public void Quit()
    {
        Application.Quit();
    }
}
