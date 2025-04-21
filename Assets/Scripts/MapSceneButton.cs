using UnityEngine;

public class MapSceneButton : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        LevelManager.Instance.LoadScene(sceneName);
        UIManager.Instance.TogglePanelsOff();
    }
}
