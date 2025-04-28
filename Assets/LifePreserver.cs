using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LifePreserver : MonoBehaviour
{
    private GameObject player;
    private Animator camAnimator;
    private Animator animator;

    public bool holeReady = false;
    public string sceneName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        camAnimator = Camera.main.GetComponent<Animator>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (holeReady && Input.GetKeyDown(KeyCode.E))
        {
            player.GetComponent<PlayerScript>().LifePreserver();
            StartCoroutine(Leave());
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

    IEnumerator Leave()
    {
        yield return new WaitForSeconds(1f);
        animator.SetTrigger("Float");

        yield return new WaitForSeconds(5f);
        camAnimator.SetBool("Fade", true);
        yield return new WaitForSeconds(.7f);
        LevelManager.Instance.LoadScene(sceneName);
    }
}
