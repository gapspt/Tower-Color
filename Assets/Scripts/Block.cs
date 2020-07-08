using UnityEngine;

public class Block : MonoBehaviour
{
    private static Material[] cachedColorMaterials = new Material[LevelSettings.BlockColors.Length];
    private static Material cachedLockedMaterial;

    public Collider blockCollider;
    public Renderer colorRenderer;
    public ParticleSystem explosionParticles;

    public float explosionRadius = 1;
    public float explosionForce = 500;

    private Rigidbody rb;
    private ParticleSystemRenderer explosionParticlesRenderer;

    private bool standing = true;

    public int ColorId { get; private set; }
    public int TowerLevel { get; private set; }

    public bool IsLocked { get; private set; } = false;
    public bool IsExploding { get; private set; } = false;

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
        explosionParticlesRenderer = explosionParticles.GetComponent<ParticleSystemRenderer>();
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

    public async void Explode()
    {
        if (IsLocked || IsExploding)
        {
            return;
        }
        IsExploding = true;

        Destroy(blockCollider);
        Destroy(colorRenderer);
        explosionParticles.Play();

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

        float particlesDuration = explosionParticles.main.duration + explosionParticles.main.startLifetime.constant;
        await TaskUtils.WaitForSeconds(this, particlesDuration);
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
        explosionParticlesRenderer.sharedMaterial = material;
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
