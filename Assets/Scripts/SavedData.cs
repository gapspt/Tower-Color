using UnityEngine;

public static class SavedData
{
    private const string GamesWonKey = "gamesWon";
    public static int GamesWon
    {
        get => PlayerPrefs.GetInt(GamesWonKey, 0);
        set => PlayerPrefs.SetInt(GamesWonKey, value);
    }

}
