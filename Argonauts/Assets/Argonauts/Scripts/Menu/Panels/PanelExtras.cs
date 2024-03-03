using System.Collections;
using System.Collections.Generic;
using HGL;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelExtras : MonoBehaviour {
    public void AdditionalCampaign()
    {
        if (ProjectSettings.I.isAllLevelsEnabled) {
            if (UserData.I.GetLastLevelPassed() >= 51) {
                TransitionManager.I.SceneToMapExtraID = TransitionManager.ID_SCENE_MENU;
                TransitionManager.I.LoadMapExtra();
            }
            else {
                PanelCommonOk panelOk = HGL_WindowManager.I.GetWindow("PanelCommonOk").GetComponent<PanelCommonOk>();
                panelOk.Init(LocalizationManager.GetLocalizedString("gui_lbl_campaign_locked"));
                HGL_WindowManager.I.OpenWindow(null, null, "PanelCommonOk", false, true);
                Ok();
            }
        }
    }

    public void Concepts() {
        TransitionManager.I.LoadConcepts();
    }

    public void Soundtracks() {
        TransitionManager.I.LoadSoundtracks();
    }

    public void Ok() {
        HGL_WindowManager.I.CloseWindow(null, null, "PanelExtras", false);
    }
}
