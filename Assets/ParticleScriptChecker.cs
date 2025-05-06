using UnityEngine;

public class ParticleScriptChecker : MonoBehaviour
{
    public string tagge;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameObject.FindGameObjectWithTag(tagge) != null)
        {
            if (GameObject.FindGameObjectWithTag(tagge).GetComponent<SkillTree>().bought == true)
            {
                GetComponent<ParticleSystem>().Play();
            }
        }
    }
}
