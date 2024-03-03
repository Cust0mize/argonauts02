using HGL;
using UnityEngine;
using UnityEngine.UI;

public class PanelGameMode : MonoBehaviour {
    [SerializeField] private Image relaxModeImage;
    [SerializeField] private Image normalModeImage;
    [SerializeField] private Image hardModeImage;

    [SerializeField] private Sprite relaxOnSprite;
    [SerializeField] private Sprite relaxOffSprite;
    [SerializeField] private Sprite normalOnSprite;
    [SerializeField] private Sprite normalOffSprite;
    [SerializeField] private Sprite hardOnSprite;
    [SerializeField] private Sprite hardOffSprite;

    public void Init() {
        UpdateState();
    }

    public void OnButtonRelaxModeClicked() {
        UserData.I.GameMode = 2;
        UpdateState();
        HGL_WindowManager.I.CloseWindow(null, null, "PanelGameMode", false);
    }

    public void OnButtonNormalModeClicked() {
        UserData.I.GameMode = 0;
        UpdateState();
        HGL_WindowManager.I.CloseWindow(null, null, "PanelGameMode", false);
    }

    public void OnButtonHardModeClicked() {
        UserData.I.GameMode = 1;
        UpdateState();
        HGL_WindowManager.I.CloseWindow(null, null, "PanelGameMode", false);
    }

    private void UpdateState() {
        switch(UserData.I.GameMode) {
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

    private void OnEnable() {
        UserData.I.GameModePanelShown = true;
    }
}
