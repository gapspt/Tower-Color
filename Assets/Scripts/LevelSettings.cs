using UnityEngine;

public class LevelSettings : ScriptableObject
{
    public const int DefaultTowerLevels = 20;
    public const int DefaultBlocksPerTowerLevel = 15;
    public const int DefaultBlockColorsNumber = 4;

    public readonly static Color[] BlockColors = {
        new Color(0, 0.5f, 0.75f),
        new Color(1, 0.75f, 0.125f),
        new Color(0.25f, 0.75f, 0),
        new Color(0.75f, 0.125f, 0),
    };

    public int towerLevels;
    public int blocksPerTowerLevel;
    public int blockColorsNumber;

    public static LevelSettings CreateInstance(int towerLevels, int blocksPerTowerLevel, int blockColorsNumber)
    {
        LevelSettings result = CreateInstance<LevelSettings>();
        result.towerLevels = towerLevels;
        result.blocksPerTowerLevel = blocksPerTowerLevel;
        result.blockColorsNumber = blockColorsNumber;
        return result;
    }

    public static LevelSettings CreateDefaultInstance() => CreateInstance(
        DefaultTowerLevels, DefaultBlocksPerTowerLevel, DefaultBlockColorsNumber);
}
