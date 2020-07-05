using UnityEngine;

public class Level : MonoBehaviour
{
    public LevelSettings settings;

    public GameObject towerPrefab;

    private void Awake()
    {
        settings = settings ?? LevelSettings.CreateDefaultInstance();
    }

    private void Start()
    {
        Tower tower = Instantiate(towerPrefab, transform).GetComponentInChildren<Tower>();
        tower.levels = settings.towerLevels;
        tower.blocksPerLevel = settings.blocksPerTowerLevel;
        tower.blockColorIds = ChooseRandomBlockColorIds(settings.blockColorsNumber);
    }

    private int[] ChooseRandomBlockColorIds(int blockColorsNumber)
    {
        if (blockColorsNumber <= 0)
        {
            return null;
        }

        int availableColorsLength = LevelSettings.BlockColors.Length;
        int[] availableNumbers = new int[availableColorsLength];
        for (int i = 0; i < availableColorsLength; i++)
        {
            availableNumbers[i] = i;
        }

        int[] result = new int[blockColorsNumber];
        for (int i = 0; i < blockColorsNumber; i++)
        {
            int j = Random.Range(i, availableColorsLength);
            result[i] = availableNumbers[j];
            availableNumbers[j] = availableNumbers[i];
        }
        return result;
    }
}
