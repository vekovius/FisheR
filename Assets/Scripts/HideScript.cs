using UnityEngine;

public class HideScript : MonoBehaviour
{
    private GameObject player;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.y > transform.position.y)
        {
            sr.sortingOrder = sr.sortingOrder = player.GetComponent<SpriteRenderer>().sortingOrder + 1;
        }
        else
        {
            sr.sortingOrder = sr.sortingOrder = player.GetComponent<SpriteRenderer>().sortingOrder - 1;
        }
    }
}
