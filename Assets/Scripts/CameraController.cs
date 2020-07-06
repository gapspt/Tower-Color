using System.Threading.Tasks;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float DefaultNextTowerLevelMovementDuration = 0.25f;

    public Camera gameCamera;

    public Vector3 lookAtPosition;
    public float distanceOffset = 20;
    public float heightOffset = 0;
    public float rotationAngle = 0;
    [Tooltip("Corresponds to the angle in degrees that the camera will rotate upon a drag from one edge of the screen to the other.")]
    public float rotationSpeed = 180;
    [Tooltip("Corresponds to the movement curve when moving from a tower level to the one below.")]
    public AnimationCurve nextTowerLevelMovementCurve = new AnimationCurve(
        new Keyframe(0, 0, 0, 1 / DefaultNextTowerLevelMovementDuration),
        new Keyframe(DefaultNextTowerLevelMovementDuration, 1, 0, 0));

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

    public async void MoveToLevelAtPosition(Vector3 newLookAtPosition)
    {
        Vector3 deltaLookAtPosition = newLookAtPosition - lookAtPosition;
        Vector3 startLookAtPosition = lookAtPosition;

        float curveDuration = nextTowerLevelMovementCurve.length > 0
            ? nextTowerLevelMovementCurve.keys[nextTowerLevelMovementCurve.length - 1].time
            : 0;
        if (curveDuration > 0)
        {
            float durationMultiplier = Mathf.Abs(deltaLookAtPosition.y);
            float invDurationMultiplier = 1 / durationMultiplier;
            float startTime = Time.unscaledTime;
            float endTime = startTime + curveDuration * durationMultiplier;

            while (true)
            {
                await TaskUtils.WaitForNextUpdate(this);
                float currentTime = Time.unscaledTime;
                if (currentTime >= endTime)
                {
                    break;
                }

                float deltaTime = (currentTime - startTime) * invDurationMultiplier;
                float value = nextTowerLevelMovementCurve.Evaluate(deltaTime);
                lookAtPosition = startLookAtPosition + deltaLookAtPosition * value;
                UpdateTransform();
            }
        }

        lookAtPosition = startLookAtPosition + deltaLookAtPosition;
        UpdateTransform();
    }

    private void UpdateTransform()
    {
        transform.position = lookAtPosition +
            (Quaternion.Euler(0, rotationAngle, 0) * new Vector3(0, heightOffset, -distanceOffset));
        transform.LookAt(lookAtPosition);
    }
}
