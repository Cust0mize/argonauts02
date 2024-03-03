using HGL;
using UnityEngine;

public class PanelRateUs : MonoBehaviour
{
    private void OnEnable()
    {
        UserData.I.SetIsRateUsPanelShowed(true);
    }

    public void OnButton1to4StarsClicked()
    {
        HGL_WindowManager.I.OpenWindow(null, null, nameof(PanelFeedback), false, true);
        HGL_WindowManager.I.CloseWindow(null, null, nameof(PanelRateUs), false);
    }

    public void OnButton5StarsClicked()
    {
#if UNITY_ANDROID
        Application.OpenURL($"market://details?id=c{Application.identifier}");
#elif UNITY_IOS
        //TODO: REPLACE THIS LINE BY IOS APP STORE URL
        Application.OpenURL("itms-apps://itunes.apple.com/app/id1591632640");
#endif
        HGL_WindowManager.I.CloseWindow(null, null, nameof(PanelRateUs), false);
    }
}
