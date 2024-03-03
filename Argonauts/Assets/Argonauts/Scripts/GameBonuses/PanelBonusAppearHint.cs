using HGL;
using UnityEngine;

public class PanelBonusAppearHint : MonoBehaviour {
    public void OnButtonOkClicked()
    {
        HGL_WindowManager.I.CloseWindow(null, null, "PanelBonusAppearHint", false);
    }
}
