using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Current { get; private set; }

    public GameObject hud;
    public GameObject lastBallTimer;
    public GameObject startScreen;
    public GameObject endScreen;

    public TMP_Text currentLevelText;
    public TMP_Text availableBallsText;
    public Image lastBallTimerFillImage;
    public Slider levelProgressSlider;
    public TMP_Text levelProgressText;
    public Button startButton;
    public Button nextLevelButton;
    public Button retryLevelButton;
    public GameObject levelWonPanel;
    public GameObject levelLostPanel;

    [Header("HUD power icons")]
    public Button rainbowPowerButton;
    public TMP_Text rainbowPowerCounterText;

    private void Awake()
    {
        Current = this;

        startButton.onClick.AddListener(OnStartButtonPressed);
        nextLevelButton.onClick.AddListener(OnNextLeveButtonPressed);
        retryLevelButton.onClick.AddListener(OnRetryLeveButtonPressed);

        rainbowPowerButton.onClick.AddListener(OnRainbowPowerButtonPressed);
    }

    public void OnStartButtonPressed()
    {
        Level.Current?.StartLevel();
    }

    public void OnNextLeveButtonPressed()
    {
        Level.Current?.LoadLevel();
    }

    public void OnRetryLeveButtonPressed()
    {
        Level.Current?.LoadLevel();
    }

    public void OnRainbowPowerButtonPressed()
    {
        Level.Current?.ActivatePower(Power.Rainbow);
    }

    public async void SetHudVisible(bool visible, bool immediately = false)
    {
        if (immediately)
        {
            hud.SetActive(visible);
        }
        else if (visible)
        {
            hud.SetActive(true);
            _ = UITransitions.FadeIn(availableBallsText);
            _ = UITransitions.ScaleIn(rainbowPowerButton);
        }
        else if (hud.activeSelf)
        {
            Task t1 = UITransitions.FadeOut(availableBallsText);
            Task t2 = UITransitions.ScaleOut(rainbowPowerButton);
            await t1;
            await t2;
            hud.SetActive(false);
        }
    }

    public async void SetLevelProgressVisible(bool visible, bool immediately = false)
    {
        if (immediately)
        {
            levelProgressSlider.gameObject.SetActive(visible);
        }
        else if (visible)
        {
            levelProgressSlider.gameObject.SetActive(true);
            _ = UITransitions.ScaleIn(levelProgressSlider);
        }
        else if (levelProgressSlider.gameObject.activeSelf)
        {
            await UITransitions.ScaleOut(levelProgressSlider);
            levelProgressSlider.gameObject.SetActive(false);
        }
    }

    public async void SetLastBallTimerVisible(bool visible, bool immediately = false)
    {
        if (immediately)
        {
            lastBallTimer.SetActive(visible);
        }
        else if (visible)
        {
            lastBallTimer.SetActive(true);
            _ = UITransitions.ScaleIn(lastBallTimer.transform, this);
        }
        else if (lastBallTimer.activeSelf)
        {
            await UITransitions.ScaleOut(lastBallTimer.transform, this);
            lastBallTimer.SetActive(false);
        }
    }

    public async void SetStartScreenVisible(bool visible, bool immediately = false)
    {
        if (immediately)
        {
            startScreen.SetActive(visible);
        }
        else if (visible)
        {
            startScreen.SetActive(true);
            _ = UITransitions.ScaleIn(startButton);
        }
        else if (startScreen.activeSelf)
        {
            await UITransitions.ScaleOut(startButton);
            startScreen.SetActive(false);
        }
    }

    public async void SetEndScreenVisible(bool visible, bool immediately = false)
    {
        if (immediately)
        {
            endScreen.SetActive(visible);
        }
        else if (visible)
        {
            endScreen.SetActive(true);
            if (levelWonPanel.activeSelf)
            {
                _ = UITransitions.ScaleIn(nextLevelButton);
            }
            else if (levelLostPanel.activeSelf)
            {
                _ = UITransitions.ScaleIn(retryLevelButton);
            }
        }
        else if (endScreen.activeSelf)
        {
            if (levelWonPanel.activeSelf)
            {
                await UITransitions.ScaleOut(nextLevelButton);
            }
            else if (levelLostPanel.activeSelf)
            {
                await UITransitions.ScaleOut(retryLevelButton);
            }
            endScreen.SetActive(false);
        }
    }

    public void HideAll()
    {
        SetHudVisible(false, true);
        SetLevelProgressVisible(false, true);
        SetLastBallTimerVisible(false, true);
        SetStartScreenVisible(false, true);
        SetEndScreenVisible(false, true);
    }

    public void UpdateLevelNumber(int level)
    {
        currentLevelText.text = $"LEVEL {level}";
    }

    public void UpdateLevelProgress(float progress)
    {
        levelProgressSlider.value = progress;
        levelProgressText.text = $"{Mathf.FloorToInt(progress * 100)}%";
    }

    public void UpdateAvailableBalls(int availableBalls)
    {
        availableBallsText.text = availableBalls.ToString();
    }

    public void UpdateAvailablePowers(IDictionary<Power, int> availablePowers)
    {
        availablePowers.TryGetValue(Power.Rainbow, out int rainbowPowers);
        rainbowPowerCounterText.text = rainbowPowers.ToString();
        rainbowPowerButton.interactable = rainbowPowers != 0;
    }

    public void UpdateLastBallTimer(float remainingTime)
    {
        lastBallTimerFillImage.fillAmount = remainingTime;
    }

    public void UpdateLevelWon(bool won)
    {
        levelWonPanel.SetActive(won);
        levelLostPanel.SetActive(!won);
    }
}
