using UnityEngine;

public class Block : MonoBehaviour
{
    public Renderer colorRenderer;

    public float explosionRadius = 1;
    public float explosionForce = 500;

    private Rigidbody rb;

    public int ColorId { get; private set; }

    public bool IsLocked { get; private set; } = false;
    public bool IsExploding { get; private set; } = false;

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
    }

    public void Setup(int colorId)
    {
        ColorId = colorId;
        colorRenderer.material.color = LevelSettings.BlockColors[colorId];
    }

    public void SetLocked(bool value)
    {
        if (IsLocked != value)
        {
            IsLocked = value;
            colorRenderer.material.color =
                value ? LevelSettings.LockedBlockColor : LevelSettings.BlockColors[ColorId];
            rb.isKinematic = value;
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

        Destroy(gameObject);
    }
}
