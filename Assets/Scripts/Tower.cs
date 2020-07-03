using UnityEngine;

public class Tower : MonoBehaviour
{
    public GameObject blockPrefab;
    public Transform blocksBase;

    public int levels = 20;
    public int blocksPerLevel = 15;

    private void Start()
    {
        float blockPlacementRadius = CalcBlockPlacementRadius(blocksPerLevel);
        float angleDiff = 360 / blocksPerLevel;
        Vector3 firstBlockPosition = new Vector3(0, 0, blockPlacementRadius);

        for (int blockLevelIndex = 0; blockLevelIndex < blocksPerLevel; blockLevelIndex++)
        {
            Vector3 evenLevelPosition = Quaternion.Euler(0, blockLevelIndex * angleDiff, 0) * firstBlockPosition;
            Vector3 oddLevelPosition = Quaternion.Euler(0, (blockLevelIndex + 0.5f) * angleDiff, 0) * firstBlockPosition;

            for (int level = 0; level < levels; level++)
            {
                Vector3 position = level % 2 == 0 ? evenLevelPosition : oddLevelPosition;
                position.y += level;
                GameObject newBlock = Instantiate(blockPrefab, transform);
                newBlock.transform.localPosition = position;
            }
        }

        float baseScale = (blockPlacementRadius + 1) * 2;
        blocksBase.localScale = new Vector3(baseScale, blocksBase.localScale.y, baseScale);
    }

    private float CalcBlockPlacementRadius(int blocks)
    {
        if (blocks > 2)
        {
            float angleA = 2 * Mathf.PI / blocks;
            float angleB = (Mathf.PI - angleA) * 0.5f;
            return Mathf.Sin(angleB) / Mathf.Sin(angleA);
        }
        else if (blocks == 2)
        {
            return 0.5f;
        }
        else
        {
            return 0;
        }
    }
}
