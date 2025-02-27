using UnityEngine;


[ExecuteAlways] //Allows code to execute in edit mode!
public class SpawnRegion : MonoBehaviour
{
    [Header("Spawn Region size")]
    public Vector2 size = new Vector2(10f, 5f); //size of spawn region
    public string speciesID; //SpeciesID
    public Color gizomoColor = Color.green; //spawn region color

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizomoColor;
        Gizmos.DrawWireCube(transform.position, size); //Draws wire cube, well square bc we passing 2d vector 
    }

    public Bounds GetBounds()
    {
        return new Bounds(transform.position, size);
    }
}
