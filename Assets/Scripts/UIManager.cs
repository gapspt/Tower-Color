﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Current { get; private set; }

    public GameObject hud;
    public GameObject startScreen;
    public GameObject endScreen;

    public TMP_Text availableBallsText;
    public Button startButton;
    public GameObject levelWonText;
    public GameObject levelLostText;

    private void Awake()
    {
        Current = this;

        startButton.onClick.AddListener(OnStartButtonPressed);
    }

    public void OnStartButtonPressed()
    {
        Level.Current?.StartLevel();
    }

    public void SetHudVisible(bool visible)
    {
        hud.gameObject.SetActive(visible);
    }

    public void SetStartScreenVisible(bool visible)
    {
        startScreen.SetActive(visible);
    }

    public void SetEndScreenVisible(bool visible)
    {
        endScreen.SetActive(visible);
    }

    public void HideAll()
    {
        SetHudVisible(false);
        SetStartScreenVisible(false);
        SetEndScreenVisible(false);
    }

    public void UpdateAvailableBalls(int availableBalls)
    {
        availableBallsText.text = availableBalls.ToString();
    }

    public void UpdateLevelWon(bool won)
    {
        levelWonText.SetActive(won);
        levelLostText.SetActive(!won);
    }
}
