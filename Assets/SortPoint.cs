using UnityEngine;

public class SortPoint : MonoBehaviour
{
    public bool locked = true;


    public void Unlock()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;

        locked = false;
    }
}
