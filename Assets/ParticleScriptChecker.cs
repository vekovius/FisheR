using UnityEngine;

public class ParticleScriptChecker : MonoBehaviour
{
    public bool useFire;
    public bool useIce;
    public bool useLightning;

    public static bool fire = false;
    public static bool ice = false;
    public static bool lightning = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (useFire)
        {
            if (fire)
            {
                GetComponent<ParticleSystem>().Play();
            }
        }
        else if (useIce)
        {
            if (ice) 
            {
                GetComponent<ParticleSystem>().Play();
            }
        }
        else if (useLightning)
        {
            if (lightning) 
            {
                GetComponent<ParticleSystem>().Play();
            }
        }
    }
}
