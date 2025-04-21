using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private Image loadingBar;

    private float target;

    private void Awake()
    {
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

    public async void LoadScene(string sceneName)
    {
        loadingBar.fillAmount = 0f;
        target = 0f; 

        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        loadingCanvas.SetActive(true);

        do
        {
            await Task.Delay(150);
            target = scene.progress;
        } while (scene.progress < 0.9f);
        target = 1f;

        await Task.Delay(500); 

        scene.allowSceneActivation = true;

        await Task.Delay(250);

        loadingCanvas.SetActive(false);
    }

    private void Update()
    {
        loadingBar.fillAmount = Mathf.MoveTowards(loadingBar.fillAmount, target, Time.deltaTime * 3f);
    }
}
