using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public AudioSource sound;
    public bool useSound = false;
    public bool fromMap = false;

    public void Switch(string name) 
    {
        if (fromMap) 
        {
            GameObject.FindGameObjectWithTag("MapPanel").SetActive(false);
        }

        if(useSound) 
        {
            sound.Play();
        }

        LevelManager.Instance.LoadScene(name);
        UIManager.Instance.TogglePanelsOff();
    }
    public void Quit()
    {
        if (useSound)
        {
            sound.Play();
        }

        Application.Quit();
    }
}
