using UnityEngine;

public class Tower : MonoBehaviour
{
    public GameObject blockPrefab;
    public GameObject towerLevelColliderPrefab;
    public Transform blocksBase;

    public int levels = 20;
    public int blocksPerLevel = 15;
    public int[] blockColorIds;

    private Block[][] blocks;

    private void Start()
    {
        Setup();
    }

    public void Setup()
    {
        if (blocks != null)
        {
            return;
        }

        int blockColorsLength = blockColorIds.Length;

        float blockPlacementRadius = CalcBlockPlacementRadius(blocksPerLevel);
        float angleDiff = 360 / blocksPerLevel;
        Vector3 firstBlockPosition = new Vector3(0, blocksBase.localPosition.y, blockPlacementRadius);

        blocks = new Block[levels][];
        for (int blockLevelIndex = 0; blockLevelIndex < blocksPerLevel; blockLevelIndex++)
        {
            Vector3 evenLevelPosition = Quaternion.Euler(0, blockLevelIndex * angleDiff, 0) * firstBlockPosition;
            Vector3 oddLevelPosition = Quaternion.Euler(0, (blockLevelIndex + 0.5f) * angleDiff, 0) * firstBlockPosition;

            for (int level = 0; level < levels; level++)
            {
                Vector3 position = level % 2 == 0 ? evenLevelPosition : oddLevelPosition;
                position.y += level;
                GameObject newBlockObject = Instantiate(blockPrefab, transform);
                newBlockObject.transform.localPosition = position;
                Block newBlock = newBlockObject.GetComponent<Block>();
                int colorId = blockColorIds[Random.Range(0, blockColorsLength)];
                newBlock.Setup(colorId, level);

                if (blockLevelIndex == 0)
                {
                    blocks[level] = new Block[blocksPerLevel];

                    GameObject levelColliderObject = Instantiate(towerLevelColliderPrefab, transform);
                    levelColliderObject.transform.localPosition = GetLevelLocalPosition(level);
                    levelColliderObject.GetComponentInChildren<TowerLevelCollider>()
                        .Setup(level, blockPlacementRadius);
                }

                blocks[level][blockLevelIndex] = newBlock;
            }
        }

        blocksBase.localScale *= (blockPlacementRadius + 1) * 2;
    }

    public Vector3 GetLevelLocalPosition(int level)
    {
        return new Vector3(0, blocksBase.localPosition.y + level, 0);
    }

    public void SetLevelLocked(int level, bool value)
    {
        Block[] levelBlocks = blocks[level];
        for (int i = 0; i < blocksPerLevel; i++)
        {
            levelBlocks[i].SetLocked(value);
        }
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
