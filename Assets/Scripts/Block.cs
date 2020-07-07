using UnityEngine;

public class Block : MonoBehaviour
{
    private static Material[] cachedColorMaterials = new Material[LevelSettings.BlockColors.Length];
    private static Material cachedLockedMaterial;

    public Renderer colorRenderer;

    public float explosionRadius = 1;
    public float explosionForce = 500;

    private Rigidbody rb;

    private bool standing = true;

    public int ColorId { get; private set; }
    public int TowerLevel { get; private set; }

    public bool IsLocked { get; private set; } = false;
    public bool IsExploding { get; private set; } = false;

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsLocked || IsExploding)
        {
            return;
        }

        if (other.GetComponentInParent<TowerLevelCollider>()?.TowerLevel < TowerLevel)
        {
            NotifyFallFromLevel();
        }
    }

    public void Setup(int colorId, int towerLevel)
    {
        ColorId = colorId;
        TowerLevel = towerLevel;

        UpdateColor();
    }

    public void SetLocked(bool value)
    {
        if (IsLocked != value)
        {
            IsLocked = value;
            rb.isKinematic = value;

            UpdateColor();
        }
    }

    public void Explode()
    {
        if (IsLocked || IsExploding)
        {
            return;
        }
        IsExploding = true;

        Vector3 center = rb.worldCenterOfMass;

        Collider[] objects = Physics.OverlapSphere(center, explosionRadius);
        foreach (Collider collider in objects)
        {
            Block otherBlock = collider.GetComponentInParent<Block>();
            if (otherBlock != null && !otherBlock.IsExploding)
            {
                if (otherBlock.ColorId == ColorId)
                {
                    otherBlock.Explode();
                }
                else
                {
                    collider.attachedRigidbody?.AddExplosionForce(explosionForce, center, explosionRadius);
                }
            }
        }

        NotifyFallFromLevel();
        Destroy(gameObject);
    }

    private void UpdateColor()
    {
        Material material;
        if (IsLocked)
        {
            material = cachedLockedMaterial;
            if (material == null)
            {
                material = new Material(colorRenderer.sharedMaterial);
                material.color = LevelSettings.LockedBlockColor;
                cachedLockedMaterial = material;
            }
        }
        else
        {
            material = cachedColorMaterials[ColorId];
            if (material == null)
            {
                material = new Material(colorRenderer.sharedMaterial);
                material.color = LevelSettings.BlockColors[ColorId];
                cachedColorMaterials[ColorId] = material;
            }
        }
        colorRenderer.sharedMaterial = material;
    }

    private void NotifyFallFromLevel()
    {
        if (standing)
        {
            standing = false;
            Level.Current?.OnBlockFell(this);
        }
    }
}
