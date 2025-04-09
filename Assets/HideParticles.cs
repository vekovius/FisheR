using UnityEngine;

public class HideParticles : MonoBehaviour
{
    private GameObject player;
    private ParticleSystemRenderer ps;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ps = GetComponent<ParticleSystemRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.y > transform.position.y)
        {
            ps.sortingOrder = player.GetComponent<SpriteRenderer>().sortingOrder + 1;
        }
        else
        {
            ps.sortingOrder = player.GetComponent<SpriteRenderer>().sortingOrder - 1;
        }
    }
}
