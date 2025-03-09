using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class FishAI : MonoBehaviour
{
    public FishType fishType; //Asigned by FishSpawner
    public Transform currentLure = null;
    public Vector2 velocity;
    public Vector2 homePosition;
  
    private float maxSpeed;
    //private float maxForce;
    private float neighborRadius;
    public float lureAttractionRadius = 3;
    private float separationDistance;
    private float alignmentWeight;
    private float cohesionWeight;
    private float separationWeight;
    private float wanderWeight;
    private float homeAttractionWeight;
    
    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if(fishType != null)
        {
            maxSpeed = fishType.maxSpeed;
            //maxForce = fishType.maxForce;
            neighborRadius = fishType.neighborRadius;
            separationDistance = fishType.separationDistance;
            alignmentWeight = fishType.alignmentWeight;
            cohesionWeight = fishType.cohesionWeight;
            separationWeight = fishType.separationWeight;
            wanderWeight = fishType.wanderWeight;
            homeAttractionWeight = fishType.homeAttractionWeight;
        }
        else
        {
            Debug.Log($"FishAI on {gameObject.name} has no FishType assigned");
        }
        velocity = new Vector2(Random.Range(-maxSpeed, maxSpeed), Random.Range(-maxSpeed, maxSpeed));
    }



    private void FixedUpdate()
    {
        Vector2 acceleration = Vector2.zero;
        acceleration += Flock();
        acceleration += Wander();
        acceleration += HomeAttraction();
        acceleration += lureAttraction();



        velocity += acceleration * Time.fixedDeltaTime;
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);

        rb.MovePosition(rb.position +  velocity * Time.fixedDeltaTime);
    }

    private Vector2 Flock()
    {
        //Collections all neighbors within neighbor Radius
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, neighborRadius);
        Vector2 alignment = Vector2.zero;
        Vector2 cohesion = Vector2.zero;
        Vector2 separation = Vector2.zero;
        int count = 0;

        //Goes though all neighbors within range
        foreach (Collider2D col in neighbors)
        {
            //
            if (col.gameObject == gameObject)
                continue;

            FishAI other = col.GetComponent<FishAI>();
            
            if (other != null)
            {
                alignment += other.velocity; //alignment will be in direction that other fish is heading
                cohesion += (Vector2)other.transform.position; //Cohestion vector is in the direction of other fish

                Vector2 diff = (Vector2)transform.position - (Vector2)other.transform.position; //diff is from current position to other fish position
                //Debug.Log($"Diff vector: {diff} Its distance is: {diff.magnitude}, the separationDistance field {separationDistance} ");
                if (diff.magnitude < separationDistance) //If the distance between other other fish and current fish is too small
                {
                    //Debug.Log($"Distance between {transform.position} and {other.transform.position} is too close!");
                    separation += diff.normalized * diff.magnitude; 
                    //Debug.Log($"Seperating in direction {separation} of magnitude {separation.magnitude}");
                }
                count++;
            }
        }
        if (count > 0)
        {
            alignment = (alignment / count).normalized * maxSpeed - velocity;
            //alignment = Vector2.ClampMagnitude(alignment, maxForce);

            cohesion = ((cohesion / count) - (Vector2)transform.position).normalized * maxSpeed - velocity;
            //cohesion = Vector2.ClampMagnitude(cohesion, maxForce);

            separation = separation.normalized * maxSpeed - velocity;
            //separation = Vector2.ClampMagnitude(separation, maxForce);

        }
        Vector2 flockVector = alignment * alignmentWeight + cohesion * cohesionWeight + separation * separationWeight;
        //Debug.Log($"Flock vector is {flockVector}");
        return alignment * alignmentWeight + cohesion * cohesionWeight + separation * separationWeight;
    }


    //Source of random direction for fish  
    private Vector2 Wander()
    {
        Vector2 wanderForce = new Vector2 (Random.Range(-1f,1f), Random.Range(-1f,1f));
        wanderForce *= wanderWeight;
        return wanderForce; //Vector2.ClampMagnitude(wanderForce, maxForce);
    }

    //This function keeps the fishies near their home point. 
    private Vector2 HomeAttraction()
    {
        Vector2 toHome = (homePosition - (Vector2)transform.position).normalized * maxSpeed - velocity;
        toHome *= homeAttractionWeight;
        return toHome; //Vector2.ClampMagnitude(toHome, maxForce);
    }

    private Vector2 lureAttraction()
    {
        if (currentLure == null)
        {
            GameObject lureObject = GameObject.FindWithTag("Lure");
            if (lureObject != null)
            {
                currentLure = lureObject.transform;
            }
        }
        else
        {
            if (currentLure.gameObject.CompareTag("OccupiedLure"))
            {
                currentLure = null;
                return Vector2.zero;
            }
        }

        if (currentLure != null)
        {
            float distanceTolure = Vector2.Distance((Vector2)transform.position, (Vector2)currentLure.position);

            if (distanceTolure <= lureAttractionRadius)
            {
                float attractionStrength = Mathf.Lerp(0.5f, 1.5f, 1 - (distanceTolure / lureAttractionRadius));
                Vector2 toLure = ((Vector2)currentLure.position - (Vector2)transform.position).normalized * maxSpeed - velocity;
                return toLure * attractionStrength;
            }
        }
        return Vector2.zero;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, neighborRadius); //Draws white circle around neigbor seeing radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, separationDistance); //Draws red circle around separtion seeing radius
    }

}
