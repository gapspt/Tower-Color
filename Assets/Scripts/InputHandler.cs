using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public CameraController cameraController;

    [Tooltip("Corresponds to the angle in degrees that the camera will rotate upon a drag from one edge of the screen to the other.")]
    public float cameraRotationSpeed = 180;

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
            hitInfo.transform.GetComponentInParent<Block>()?.Explode();
        }
    }
}
