using UnityEngine;

public class LureStateController : MonoBehaviour
{
    private bool isOccupied = false;
    private GameObject hookedFish;

    public bool IsOccupied => isOccupied;
    public GameObject HookedFish => hookedFish;

    public void SetOccupied(GameObject fish)
    {
        isOccupied = true;
        hookedFish = fish;

        gameObject.tag = "OccupiedLure";
    }

    public void SetFree()
    {
        isOccupied = false;
        hookedFish = null;

        gameObject.tag = "Lure";
    }
}
