using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    private static GameObject managerInstance;
    public GameObject panel;
    private bool open = false;

    public AudioSource clip;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (managerInstance == null)
        {
            managerInstance = gameObject;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        open = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)) 
        {
            clip.Play();

            if (!open) 
            {
                panel.SetActive(true);
                open = true;
            }
            else 
            {
                panel.SetActive(false);
                open = false;
            }
        }

        if(GetComponent<Canvas>().worldCamera == null) 
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
        }

        transform.position = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y);
    }
}
