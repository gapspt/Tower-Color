using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera gameCamera;

    public Vector3 lookAtPosition;
    public float distanceOffset = 20;
    public float heightOffset = 8;

    private void Start()
    {
        SetRotationAngle(0);
    }

    public void SetRotationAngle(float angle)
    {
        transform.position = lookAtPosition +
            (Quaternion.Euler(0, angle, 0) * new Vector3(0, heightOffset, -distanceOffset));
        transform.LookAt(lookAtPosition);
    }
}
