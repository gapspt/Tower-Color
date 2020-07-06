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
    [Tooltip("Corresponds to the movement curve when starting the game (the duration is multiplied by the number of tower levels).")]
    public AnimationCurve startMovementCurve = AnimationCurve
        .EaseInOut(0, 0, DefaultNextTowerLevelMovementDuration, 1);
    [Tooltip("Corresponds to the angle in degrees that the camera will rotate when starting the game (per tower level).")]
    public float startRotationSpeed = 30;


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

    public Task MoveToLevelAtPosition(Vector3 newLookAtPosition)
    {
        Vector3 deltaLookAtPosition = newLookAtPosition - lookAtPosition;
        return AnimateLookAtAndRotation(deltaLookAtPosition, 0, nextTowerLevelMovementCurve);
    }

    public Task MoveAtLevelStart(Vector3 newLookAtPosition)
    {
        Vector3 deltaLookAtPosition = newLookAtPosition - lookAtPosition;
        float deltaRotation = startRotationSpeed * Mathf.Abs(deltaLookAtPosition.y);
        return AnimateLookAtAndRotation(deltaLookAtPosition, deltaRotation, startMovementCurve);
    }

    private async Task AnimateLookAtAndRotation(Vector3 deltaLookAtPosition, float deltaRotation, AnimationCurve curve)
    {
        Vector3 startLookAtPosition = lookAtPosition;
        float startRotationAngle = rotationAngle;

        float curveDuration = curve.length > 0
            ? curve.keys[curve.length - 1].time
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
                float value = curve.Evaluate(deltaTime);
                lookAtPosition = startLookAtPosition + deltaLookAtPosition * value;
                if (deltaRotation != 0)
                {
                    rotationAngle = (startRotationAngle + value * deltaRotation) % 360;
                }
                UpdateTransform();
            }
        }

        lookAtPosition = startLookAtPosition + deltaLookAtPosition;
        if (deltaRotation != 0)
        {
            rotationAngle = (startRotationAngle + deltaRotation) % 360;
        }
        UpdateTransform();
    }

    private void UpdateTransform()
    {
        transform.position = lookAtPosition +
            (Quaternion.Euler(0, rotationAngle, 0) * new Vector3(0, heightOffset, -distanceOffset));
        transform.LookAt(lookAtPosition);
    }
}
