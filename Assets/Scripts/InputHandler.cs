using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private bool dragging = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        // It's only needed so that the OnPointerUp works
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (dragging)
        {
            Level.Current?.OnDrag(eventData.delta);
            dragging = false;
        }
        else
        {
            Level.Current?.OnClick(eventData.position);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragging = true;
        Level.Current?.OnDrag(eventData.delta);
    }
}
