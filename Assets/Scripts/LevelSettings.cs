﻿using UnityEngine;

public class LevelSettings : ScriptableObject
{
    public const int WaterLayer = 4;
    public const int TowerBlocksLayer = 8;

    public const int DefaultTowerLevels = 20;
    public const int DefaultTowerUnlockedLevels = 8;
    public const int DefaultBlocksPerTowerLevel = 15;
    public const int DefaultBlockColorsNumber = 4;
    public const int DefaultAvailableBalls = 26;
    public const int DefaultWinMaxStandingBlocks = 30;
    public const float DefaultLoseDelay = 3;

    public readonly static Color[] BlockColors = {
        new Color32(115, 180, 255, 255),
        new Color32(255, 215, 40, 255),
        new Color32(255, 113, 113, 255),
        new Color32(150, 100, 240, 255),
        new Color32(255, 115, 255, 255),
        new Color32(100, 255, 190, 255),
    };
    public readonly static Color LockedBlockColor = new Color32(60, 60, 60, 255);

    public int towerLevels;
    public int towerUnlockedLevels;
    public int blocksPerTowerLevel;
    public int blockColorsNumber;
    public int availableBalls;
    public int winMaxStandingBlocks;
    public float loseDelay;

    public static LevelSettings CreateInstance(int towerLevels, int towerUnlockedLevels,
        int blocksPerTowerLevel, int blockColorsNumber, int availableBalls,
        int winMaxStandingBlocks, float loseDelay)
    {
        LevelSettings result = CreateInstance<LevelSettings>();
        result.towerLevels = towerLevels;
        result.towerUnlockedLevels = towerUnlockedLevels;
        result.blocksPerTowerLevel = blocksPerTowerLevel;
        result.blockColorsNumber = blockColorsNumber;
        result.availableBalls = availableBalls;
        result.winMaxStandingBlocks = winMaxStandingBlocks;
        result.loseDelay = loseDelay;
        return result;
    }

    public static LevelSettings CreateDefaultInstance() => CreateInstance(
        DefaultTowerLevels, DefaultTowerUnlockedLevels, DefaultBlocksPerTowerLevel,
        DefaultBlockColorsNumber, DefaultAvailableBalls,
        DefaultWinMaxStandingBlocks, DefaultLoseDelay);
}
