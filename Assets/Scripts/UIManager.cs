using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Current { get; private set; }

    public GameObject startScreen;

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

    public void SetStartScreenVisible(bool visible)
    {
        startScreen.SetActive(visible);
    }

    public void HideAll()
    {
        SetStartScreenVisible(false);
    }
}
