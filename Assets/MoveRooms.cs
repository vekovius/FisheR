using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveRooms : MonoBehaviour
{
    public Transform spawnPosObject;
    private Animator camAnimator;
    private GameObject player;

    private Vector2 spawnPos;

    private bool inUse = false;

    void Start()
    {
        inUse = false;
        camAnimator = Camera.main.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        spawnPos = new Vector2(spawnPosObject.transform.position.x, spawnPosObject.transform.position.y);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (Input.GetAxis("Submit") != 0 && !inUse)
            {
                inUse = true;
                StartCoroutine(Teleport());
            }
        }
    }

     IEnumerator Teleport()
    {
        camAnimator.SetBool("Fade", true);
        yield return new WaitForSeconds(.8f);

        player.transform.position = spawnPos;
        Camera.main.transform.position = new Vector3(spawnPos.x, spawnPos.y, -10);

        camAnimator.SetBool("Fade", false);
        inUse = false;
    }
}
