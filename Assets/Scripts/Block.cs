using UnityEngine;

public class Block : MonoBehaviour
{
    public Renderer colorRenderer;

    private void OnMouseUpAsButton()
    {
        Destroy(gameObject);
    }

    public void SetColor(Color color)
    {
        colorRenderer.material.color = color;
    }
}
