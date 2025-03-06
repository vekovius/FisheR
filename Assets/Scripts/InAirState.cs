using UnityEngine;

public class InAirState : StateInterface
{
    private GameObject lure;
    private float waterLevel;

    public InAirState(float waterLevel)
    {
        this.waterLevel = waterLevel;
    }

    public void Enter()
    {
        lure = GameObject.FindWithTag("Lure");
        
        if ( lure == null )
        {
            Debug.Log("No lure found!");
        }
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }

    public bool IsLureInWater()
    {
        if (lure == null) return false;
        return lure.transform.position.y < waterLevel;
    }
}
