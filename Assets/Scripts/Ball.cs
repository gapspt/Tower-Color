using UnityEngine;

public class Ball : MonoBehaviour
{
    private static Material[] cachedColorMaterials = new Material[LevelSettings.BlockColors.Length];

    public Renderer colorRenderer;

    public Material rainbowMaterial;

    public int ColorId { get; private set; }

    public Power power = Power.None;

    public void Setup(int colorId)
    {
        ColorId = colorId;

        UpdateColor();
    }

    public void SetPower(Power power)
    {
        this.power = power;

        UpdateColor();
    }

    private void UpdateColor()
    {
        Material material;
        switch (power)
        {
            case Power.Rainbow:
                material = rainbowMaterial;
                break;
            case Power.None:
            default:
                material = cachedColorMaterials[ColorId];
                if (material == null)
                {
                    material = new Material(colorRenderer.sharedMaterial);
                    material.color = LevelSettings.BlockColors[ColorId];
                    cachedColorMaterials[ColorId] = material;
                }
                break;
        }

        colorRenderer.sharedMaterial = material;
    }
}
