using UnityEngine;

public class LevelSettings : ScriptableObject
{
    public const int DefaultTowerLevels = 20;
    public const int DefaultBlocksPerTowerLevel = 15;
    public const int DefaultBlockColorsNumber = 4;
    public const int DefaultAvailableBalls = 26;

    public readonly static Color[] BlockColors = {
        new Color(0, 0.5f, 0.75f),
        new Color(1, 0.75f, 0.125f),
        new Color(0.25f, 0.75f, 0),
        new Color(0.75f, 0.125f, 0),
    };

    public int towerLevels;
    public int blocksPerTowerLevel;
    public int blockColorsNumber;
    public int availableBalls;

    public static LevelSettings CreateInstance(int towerLevels,
        int blocksPerTowerLevel, int blockColorsNumber, int availableBalls)
    {
        LevelSettings result = CreateInstance<LevelSettings>();
        result.towerLevels = towerLevels;
        result.blocksPerTowerLevel = blocksPerTowerLevel;
        result.blockColorsNumber = blockColorsNumber;
        result.availableBalls = availableBalls;
        return result;
    }

    public static LevelSettings CreateDefaultInstance() => CreateInstance(
        DefaultTowerLevels, DefaultBlocksPerTowerLevel,
        DefaultBlockColorsNumber, DefaultAvailableBalls);
}
