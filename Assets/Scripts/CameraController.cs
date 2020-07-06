using System.Threading.Tasks;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera gameCamera;

    public Vector3 lookAtPosition;
    public float distanceOffset = 20;
    public float heightOffset = 0;
    public float rotationAngle = 0;
    [Tooltip("Corresponds to the angle in degrees that the camera will rotate upon a drag from one edge of the screen to the other.")]
    public float rotationSpeed = 180;
    [Tooltip("Corresponds to the speed when moving to another tower level position.")]
    public float nextTowerLevelSpeed = 5;

    private void Start()
    {
        UpdateTransform();
    }

    public void SetLookAtPosition(Vector3 lookAtPosition)
    {
        this.lookAtPosition = lookAtPosition;
        UpdateTransform();
    }

    public void RotateByRelativeAmount(float relativeAmount)
    {
        rotationAngle = (rotationAngle + relativeAmount * rotationSpeed) % 360;
        UpdateTransform();
    }

    public async void MoveToLevelAtPosition(Vector3 lookAtPosition)
    {
        // TODO: Start an animation and wait for it to end
        float duration = Mathf.Abs(this.lookAtPosition.y - lookAtPosition.y) / nextTowerLevelSpeed;
        await TaskUtils.WaitForSecondsRealtime(this, duration);

        this.lookAtPosition = lookAtPosition;
        UpdateTransform();
    }

    private void UpdateTransform()
    {
        transform.position = lookAtPosition +
            (Quaternion.Euler(0, rotationAngle, 0) * new Vector3(0, heightOffset, -distanceOffset));
        transform.LookAt(lookAtPosition);
    }
}
