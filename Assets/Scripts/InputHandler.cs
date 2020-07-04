using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public CameraController cameraController;

    public GameObject ballPrefab;

    [Tooltip("Corresponds to the angle in degrees that the camera will rotate upon a drag from one edge of the screen to the other.")]
    public float cameraRotationSpeed = 180;

    public float throwSpeed = 35;
    public float throwGravityMultiplier = 2;
    public Vector3 throwCameraPositionOffset;

    private bool dragging = false;
    private float cameraAngle = 0;

    public void OnPointerDown(PointerEventData eventData)
    {
        // It's only needed so that the OnPointerUp works
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (dragging)
        {
            cameraAngle = (cameraAngle + CalcCameraDeltaAngle(eventData)) % 360;
            cameraController.SetRotationAngle(cameraAngle);
            dragging = false;
        }
        else
        {
            CheckClickOnBlock(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragging = true;
        cameraController.SetRotationAngle(cameraAngle + CalcCameraDeltaAngle(eventData));
    }

    public float CalcCameraDeltaAngle(PointerEventData eventData)
    {
        return (eventData.position.x - eventData.pressPosition.x) / Screen.width * cameraRotationSpeed;
    }

    public void CheckClickOnBlock(PointerEventData eventData)
    {
        Ray ray = cameraController.gameCamera.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Block block = hitInfo.transform.GetComponentInParent<Block>();
            if (block != null)
            {
                ThrowBall(block, hitInfo.point);
            }
        }
    }

    private async void ThrowBall(Block block, Vector3 hitPosition)
    {
        float gravity = Physics.gravity.y * throwGravityMultiplier;

        Vector3 initialBallPosition = cameraController.gameCamera.transform.TransformPoint(throwCameraPositionOffset);
        GameObject ball = Instantiate(ballPrefab, initialBallPosition, Quaternion.identity);

        Vector3 distance = hitPosition - initialBallPosition;
        Vector2 horizontalDistance = new Vector2(distance.x, distance.z);
        float totalTime = horizontalDistance.magnitude / throwSpeed;
        float verticalSpeed = distance.y / totalTime - gravity * totalTime * 0.5f;

        float startTime = Time.time;
        float endTime = startTime + totalTime;
        Vector2 horizontalDirection = horizontalDistance.normalized;

        await TaskUtils.WaitForNextUpdate(this);
        while (Time.time < endTime)
        {
            float t = Time.time - startTime;
            float verticalDiff = (gravity * t * 0.5f + verticalSpeed) * t;
            Vector2 horizontalDiff = horizontalDirection * (throwSpeed * t);

            ball.transform.position = initialBallPosition + new Vector3(horizontalDiff.x, verticalDiff, horizontalDiff.y);

            await TaskUtils.WaitForNextUpdate(this);
        }

        Destroy(ball);
        block.Explode();
    }
}
