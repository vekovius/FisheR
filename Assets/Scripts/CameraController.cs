using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    public Transform target;

    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Smoothing")]
    [Range(0,2)]
    public float smoothSpeed = 0.125f;

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;

            Vector3 smoothedPosition = Vector3.Lerp(desiredPosition, offset, smoothSpeed);

            transform.position = smoothedPosition;
        }
    }
}
