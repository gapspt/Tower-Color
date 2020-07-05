using UnityEngine;

public class Ball : MonoBehaviour
{
    public Renderer colorRenderer;

    public int ColorId { get; private set; }

    public void Setup(int colorId)
    {
        ColorId = colorId;
        colorRenderer.material.color = LevelSettings.BlockColors[colorId];
    }
}
