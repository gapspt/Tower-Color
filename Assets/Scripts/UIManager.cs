using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Current { get; private set; }

    public GameObject hud;
    public GameObject startScreen;
    public GameObject endScreen;

    public TMP_Text availableBallsText;
    public Slider levelProgressSlider;
    public TMP_Text levelProgressText;
    public Button startButton;
    public Button nextLevelButton;
    public Button retryLevelButton;
    public GameObject levelWonPanel;
    public GameObject levelLostPanel;

    private void Awake()
    {
        Current = this;

        startButton.onClick.AddListener(OnStartButtonPressed);
        nextLevelButton.onClick.AddListener(OnNextLeveButtonPressed);
        retryLevelButton.onClick.AddListener(OnRetryLeveButtonPressed);
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

    public void SetHudVisible(bool visible)
    {
        hud.gameObject.SetActive(visible);
    }

    public void SetLevelProgressVisible(bool visible)
    {
        levelProgressSlider.gameObject.SetActive(visible);
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
        SetLevelProgressVisible(false);
        SetStartScreenVisible(false);
        SetEndScreenVisible(false);
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

    public void UpdateLevelWon(bool won)
    {
        levelWonPanel.SetActive(won);
        levelLostPanel.SetActive(!won);
    }
}
