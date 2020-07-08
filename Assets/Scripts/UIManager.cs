using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Current { get; private set; }

    public GameObject hud;
    public GameObject startScreen;

    public TMP_Text availableBallsText;
    public Button startButton;

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

    public void HideAll()
    {
        SetHudVisible(false);
        SetStartScreenVisible(false);
    }

    public void UpdateAvailableBalls(int availableBalls)
    {
        availableBallsText.text = availableBalls.ToString();
    }
}
