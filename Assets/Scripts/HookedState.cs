using UnityEngine;

public class HookedState : StateInterface
{
    public void Enter()
    {
        Debug.Log("Entered hooked state");
    }

    public void Exit()
    {
        Debug.Log("Exited hooked state");
    }

    void Start()
    {
        
    }

    public void Update()
    {
        
    }

}
