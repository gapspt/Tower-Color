using UnityEngine;

public class TowerLevelCollider : MonoBehaviour
{
    public BoxCollider blocksCollider;

    public int TowerLevel { get; private set; }

    public void Setup(int towerLevel, float blockPlacementRadius)
    {
        TowerLevel = towerLevel;

        blockPlacementRadius *= 100;
        Vector3 colliderSize = blocksCollider.size;
        colliderSize.x *= blockPlacementRadius;
        colliderSize.z *= blockPlacementRadius;
        blocksCollider.size = colliderSize;
    }
}
