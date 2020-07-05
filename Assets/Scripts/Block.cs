using UnityEngine;

public class Block : MonoBehaviour
{
    public Renderer colorRenderer;

    public float explosionRadius = 1;
    public float explosionForce = 500;

    public int ColorId { get; private set; }

    public bool IsExploding { get; private set; } = false;

    public void SetColor(int colorId)
    {
        ColorId = colorId;
        colorRenderer.material.color = LevelSettings.BlockColors[colorId];
    }

    public void Explode()
    {
        if (IsExploding)
        {
            return;
        }
        IsExploding = true;

        Vector3 center = GetComponentInChildren<Rigidbody>()?.worldCenterOfMass ?? transform.position;

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
