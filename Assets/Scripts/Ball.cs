using UnityEngine;

public class Ball : MonoBehaviour
{
    private static Material[] cachedColorMaterials = new Material[LevelSettings.BlockColors.Length];

    public Renderer colorRenderer;

    public int ColorId { get; private set; }

    public void Setup(int colorId)
    {
        ColorId = colorId;

        UpdateColor();
    }

    private void UpdateColor()
    {
        Material material = cachedColorMaterials[ColorId];
        if (material == null)
        {
            material = new Material(colorRenderer.sharedMaterial);
            material.color = LevelSettings.BlockColors[ColorId];
            cachedColorMaterials[ColorId] = material;
        }
        colorRenderer.sharedMaterial = material;
    }
}
