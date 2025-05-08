using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mudhole : MonoBehaviour
{
    private GameObject player;
    private Animator animator;

    public bool holeReady = false;
    public string sceneName;

    public ParticleSystem firstPs;
    public ParticleSystem secondPs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (holeReady && Input.GetKeyDown(KeyCode.E))
        {
            player.GetComponent<PlayerScript>().leftBoat = false;
            StartCoroutine(MudJump());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            holeReady = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            holeReady = false;
        }
    }

    IEnumerator MudJump() 
    {
        animator.SetTrigger("Muddin");

        firstPs.Play();
        yield return new WaitForSeconds(2f);
        firstPs.Stop();
        GetComponent<AudioSource>().Play();
        secondPs.Play();
        yield return new WaitForSeconds(4f);
        LevelManager.Instance.LoadScene(sceneName);
    }
}
