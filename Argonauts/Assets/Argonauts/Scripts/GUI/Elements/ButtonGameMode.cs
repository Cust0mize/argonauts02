using UnityEngine;
using UnityEngine.UI;

public class ButtonGameMode : MonoBehaviour {
    [SerializeField] private Image relaxModeImage;
    [SerializeField] private Image normalModeImage;
    [SerializeField] private Image hardModeImage;

    [SerializeField] private Sprite relaxOnSprite;
    [SerializeField] private Sprite relaxOffSprite;
    [SerializeField] private Sprite normalOnSprite;
    [SerializeField] private Sprite normalOffSprite;
    [SerializeField] private Sprite hardOnSprite;
    [SerializeField] private Sprite hardOffSprite;

    private void OnEnable() {
        UserData.I.OnGameModeChanged += OnGameModeChanged;
        UpdateState();
    }

    private void OnDisable() {
        UserData.I.OnGameModeChanged -= OnGameModeChanged;
    }

    private void OnGameModeChanged(int gameMode) {
        UpdateState();
    }

    private void UpdateState() {
        switch (UserData.I.GameMode) {
            case 1:
                relaxModeImage.sprite = relaxOffSprite;
                normalModeImage.sprite = normalOffSprite;
                hardModeImage.sprite = hardOnSprite;
                break;
            case 2:
                relaxModeImage.sprite = relaxOnSprite;
                normalModeImage.sprite = normalOffSprite;
                hardModeImage.sprite = hardOffSprite;
                break;
            default:
                relaxModeImage.sprite = relaxOffSprite;
                normalModeImage.sprite = normalOnSprite;
                hardModeImage.sprite = hardOffSprite;
                break;
        }
    }
}
