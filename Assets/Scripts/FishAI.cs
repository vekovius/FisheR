using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class FishAI : MonoBehaviour
{
    public FishType fishType; //Asigned by FishSpawner
    public Vector2 velocity;
    public Vector2 homePosition;

    private float maxSpeed;
    private float maxForce;
    private float neighborRadius;
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
            maxForce = fishType.maxForce;
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

        velocity += acceleration * Time.fixedDeltaTime;
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);

        rb.MovePosition(rb.position +  velocity * Time.fixedDeltaTime);
    }

    private Vector2 Flock()
    {
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, neighborRadius);
        Vector2 alignment = Vector2.zero;
        Vector2 cohesion = Vector2.zero;
        Vector2 separation = Vector2.zero;
        int count = 0;

        foreach (Collider2D col in neighbors)
        {
            if (col.gameObject == gameObject)
                continue;

            FishAI other = col.GetComponent<FishAI>();
            if (other != null)
            {
                alignment += other.velocity;
                cohesion += (Vector2)other.transform.position;

                Vector2 diff = (Vector2)transform.position - (Vector2)other.transform.position;
                if (diff.magnitude < separationDistance)
                {
                    separation += diff.normalized / diff.magnitude;
                }
                count++;
            }
        }

        if (count > 0)
        {
            alignment = (alignment / count).normalized * maxSpeed - velocity;
            alignment = Vector2.ClampMagnitude(alignment, maxForce);

            cohesion = ((cohesion / count) - (Vector2)transform.position).normalized * maxSpeed - velocity;
            cohesion = Vector2.ClampMagnitude(cohesion, maxForce);

            separation = separation.normalized * maxSpeed - velocity;
            separation = Vector2.ClampMagnitude(separation, maxForce);

        }

        return alignment * alignmentWeight + cohesion * cohesionWeight + separation * separationWeight;
    }

    private Vector2 Wander()
    {
        Vector2 wanderForce = new Vector2 (Random.Range(-1f,1f), Random.Range(-1f,1f));
        return wanderForce.normalized;
    }


    private Vector2 HomeAttraction()
    {
        Vector2 toHome = (homePosition - (Vector2)transform.position).normalized * maxSpeed - velocity;
        return Vector2.ClampMagnitude(toHome, maxForce);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, neighborRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, separationDistance);
    }
}
