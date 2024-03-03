using System.Collections;
using System.Collections.Generic;
using HGL;
using UnityEngine;
using UnityEngine.Analytics;

public class PanelPause : MonoBehaviour {

    private void OnEnable() {
        GameManager.I.IsPause = true;
    }

    private void OnDisable() {
        GameManager.I.IsPause = false;
    }

    public void OnButtonContinueClick() {
		GameManager.I.IsPause = false;
		HGL_WindowManager.I.CloseWindow(null, null, "PanelPause", false);
	}

	public void OnButtonRestartClick() {
		PanelYesNo panel = HGL_WindowManager.I.GetWindow("PanelYesNo").GetComponent<PanelYesNo>();
		panel.Init(LocalizationManager.GetLocalizedString("gui_lbl_restart_request"));
		panel.OnResultClose += OnResultRestartRequest;
		HGL_WindowManager.I.OpenWindow(null, null, "PanelYesNo", false, true);
	}

	public void OnButtonSettingsClick() {
		HGL_WindowManager.I.OpenWindow(null, null, "PanelSettings", false, true);
	}

	public void OnButtonMapClick() {
		PanelYesNo panel = HGL_WindowManager.I.GetWindow("PanelYesNo").GetComponent<PanelYesNo>();
		panel.Init(LocalizationManager.GetLocalizedString("gui_lbl_go_map_request"));
		panel.OnResultClose += OnResultMapRequest;
		HGL_WindowManager.I.OpenWindow(null, null, "PanelYesNo", false, true);
	}

	public void OnButtonMenuClick() {
		PanelYesNo panel = HGL_WindowManager.I.GetWindow("PanelYesNo").GetComponent<PanelYesNo>();
		panel.Init(LocalizationManager.GetLocalizedString("gui_lbl_go_menu_request"));
		panel.OnResultClose += OnResultMenuRequest;
		HGL_WindowManager.I.OpenWindow(null, null, "PanelYesNo", false, true);
	}

	private void OnResultRestartRequest(bool result) {
		PanelYesNo panel = HGL_WindowManager.I.GetWindow("PanelYesNo").GetComponent<PanelYesNo>();
		panel.OnResultClose -= OnResultRestartRequest;

		if (result) {
			TransitionManager.I.RestartLevel();
		}
	}

	private void OnResultMapRequest(bool result) {
		PanelYesNo panel = HGL_WindowManager.I.GetWindow("PanelYesNo").GetComponent<PanelYesNo>();
		panel.OnResultClose -= OnResultMapRequest;

		if (result) {
            OnPlayerLeaveLevel();

            if (GameManager.I.LevelNumber < 51) TransitionManager.I.LoadMap();
            else TransitionManager.I.LoadMapExtra();
        }
	}

	private void OnResultMenuRequest(bool result) {
		PanelYesNo panel = HGL_WindowManager.I.GetWindow("PanelYesNo").GetComponent<PanelYesNo>();
		panel.OnResultClose -= OnResultMenuRequest;

		if (result) {
            OnPlayerLeaveLevel();
			TransitionManager.I.LoadMenu();
		}
	}

    private void OnPlayerLeaveLevel() {
        AnalyticsEvent.Custom("level_quit", new Dictionary<string, object> {
            { "level_number", GameManager.I.LevelNumber }
        });
    }
}
