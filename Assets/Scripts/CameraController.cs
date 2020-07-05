using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera gameCamera;

    public Vector3 lookAtPosition;
    public float distanceOffset = 20;
    public float heightOffset = 8;
    public float rotationAngle = 0;
    [Tooltip("Corresponds to the angle in degrees that the camera will rotate upon a drag from one edge of the screen to the other.")]
    public float rotationSpeed = 180;

    private void Start()
    {
        SetRotationAngle(rotationAngle);
    }

    public void SetRotationAngle(float angle)
    {
        rotationAngle = angle;
        transform.position = lookAtPosition +
            (Quaternion.Euler(0, angle, 0) * new Vector3(0, heightOffset, -distanceOffset));
        transform.LookAt(lookAtPosition);
    }

    public void RotateByRelativeAmount(float relativeAmount)
    {
        SetRotationAngle((rotationAngle + relativeAmount * rotationSpeed) % 360);
    }
}
