using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using HGL;

public class CampaignManager : MonoBehaviour {
    [SerializeField] private Button campaignButton;

    private void Awake()
    {
        campaignButton.interactable = ProjectSettings.I.isAllLevelsEnabled;
    }

    public void OnCampaignButtonClicked()
    {
        if (MapManager.I.ScrollingEnable) {
            if (MapManager.I.ScrollingPanel.CanClickLevel) {

                if (UserData.I.GetLastLevelPassed() >= 51) {
                    TransitionManager.I.SceneToMapExtraID = TransitionManager.ID_SCENE_MAP;
                    TransitionManager.I.LoadMapExtra();
                } else {
                    PanelCommonOk panelOk = HGL_WindowManager.I.GetWindow("PanelCommonOk").GetComponent<PanelCommonOk>();
                    panelOk.Init(LocalizationManager.GetLocalizedString("gui_lbl_campaign_locked"));
                    HGL_WindowManager.I.OpenWindow(null, null, "PanelCommonOk", false, true);
                }
            }

        }
    }
}
