using System;
using System.Collections;
using System.Collections.Generic;
using HGL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yodo1.MAS;

public class MenuManager : LocalSingletonBehaviour<MenuManager> {
    [SerializeField] private UITweener[] tweeners;

	[SerializeField] private GameObject buttonTrophy;

    [SerializeField] private GameObject extrasButton;

    [SerializeField] private TextMeshProUGUI copyrightText;

	protected override void Awake() {
        copyrightText.text = ProjectSettings.I.copyright;

		buttonTrophy.gameObject.SetActive(ProjectSettings.I.isTrophiesEnabled);
		//extrasButton.gameObject.SetActive(ProjectSettings.I.isBFG_Collector);
		extrasButton.gameObject.SetActive(false);

		PlayerProfileManager.I.GetType();
	}

	private IEnumerator Start() {
		FadeManager.I.ForceFadeIn();
		yield return new WaitForSeconds(0.1F);
		yield return FadeManager.I.FadeOut(2F);

		foreach (UITweener tw in tweeners) {
			tw.PlayForward();
		}

		yield return new WaitForSeconds(0.3F);

		if (PlayerProfileManager.I.GetCurrentProfile() == null) {
			PlayerProfileManager.I.CreateProfile("Default2");
			PlayerProfileManager.I.SetCurrentProfile("Default2");
			PlayerProfileManager.I.SaveAll();
		}

		Yodo1U3dMasCallback.OnSdkInitializedEvent += (success, error) =>
		{
			Debug.Log("[Yodo1 Mas] OnSdkInitializedEvent, success:" + success + ", error: " + error);
		};

		Yodo1AdBuildConfig config = new Yodo1AdBuildConfig().enableUserPrivacyDialog(true);
		Yodo1U3dMas.SetAdBuildConfig(config);
		Yodo1U3dMas.InitializeSdk();
	}

	public void Play() {
		if (PlayerProfileManager.I.GetCurrentProfile() == null) {
			//PanelEnterName panel = HGL_WindowManager.I.GetWindow("PanelEnterName").GetComponent<PanelEnterName>();
			//panel.OnEnterNameSuccess += OnEnterNameSuccessAndPlay;
			//panel.OnEnterNameClose += OnEnterNameClose;
			//panel.Init();
			//HGL_WindowManager.I.OpenWindow(null, null, "PanelEnterName", false, true);


			PlayerProfileManager.I.CreateProfile("Default2");
			PlayerProfileManager.I.SetCurrentProfile("Default2");
			PlayerProfileManager.I.SaveAll();
		} else {
			StartCoroutine(IEWaitButtonsHide(TransitionManager.I.LoadMap));
		}
	}

	private void OnEnterNameClose() {
		PanelEnterName panel = HGL_WindowManager.I.GetWindow("PanelEnterName").GetComponent<PanelEnterName>();
		panel.OnEnterNameSuccess -= OnEnterNameSuccess;
		panel.OnEnterNameClose -= OnEnterNameClose;
	}

	private void OnEnterNameSuccess(string profileName) {
		PanelEnterName panel = HGL_WindowManager.I.GetWindow("PanelEnterName").GetComponent<PanelEnterName>();
		panel.OnEnterNameSuccess -= OnEnterNameSuccess;
		panel.OnEnterNameClose -= OnEnterNameClose;

		if (PlayerProfileManager.I.PlayerExist(profileName)) {
			PanelCommonOk panelOk = HGL_WindowManager.I.GetWindow("PanelCommonOk").GetComponent<PanelCommonOk>();
			panelOk.Init(LocalizationManager.GetLocalizedString("menu_lbl_player_exist"));
			HGL_WindowManager.I.OpenWindow(null, null, "PanelCommonOk", false, true);
		} else {
			PlayerProfileManager.I.CreateProfile(profileName);
			PlayerProfileManager.I.SetCurrentProfile(profileName);
			PlayerProfileManager.I.SaveAll();
		}
	}

	private void OnEnterNameSuccessAndPlay(string profileName) {
		OnEnterNameSuccess(profileName);
		Play();
	}

	public void Exit() {
		PanelYesNo panel = HGL_WindowManager.I.GetWindow("PanelYesNo").GetComponent<PanelYesNo>();
		panel.Init(LocalizationManager.GetLocalizedString("menu_lbl_confirm_exit"));
		panel.OnResultClose += OnResultExit;
		HGL_WindowManager.I.OpenWindow(null, null, "PanelYesNo", false, true);
	}

	private void OnResultExit(bool result) {
		PanelYesNo panel = HGL_WindowManager.I.GetWindow("PanelYesNo").GetComponent<PanelYesNo>();
		panel.OnResultClose -= OnResultExit;

		if (result) {
#if UNITY_EDITOR

#else
	StartCoroutine(IEWaitButtonsHide(Application.Quit));
#endif
		}
	}

    public void Trophies() {
        AwardScreenManager.IsOpenedFromMap = false;
        TransitionManager.I.LoadTrophies();
    }

	public void Credits() {
		HGL_WindowManager.I.OpenWindow(null, null, "PanelCredits", false, true);
	}

	public void Settings() {
		HGL_WindowManager.I.OpenWindow(null, null, "PanelSettings", false, true);
	}

	public void ChangePlayer() {
		PanelSelectPlayer panel = HGL_WindowManager.I.GetWindow("PanelSelectPlayer").GetComponent<PanelSelectPlayer>();
		panel.Init();
		HGL_WindowManager.I.OpenWindow(null, null, "PanelSelectPlayer", false, true);
	}

    public void Extras() {
        HGL_WindowManager.I.OpenWindow(null, null, "PanelExtras", false, true);
    }

	private IEnumerator IEWaitButtonsHide(Action callback) {
		float maxTime = 0f;
		foreach (UITweener tw in tweeners) {
			maxTime = Mathf.Max(maxTime, tw.duration);
		}
		foreach (UITweener tw in tweeners) {
			tw.PlayReverse();
		}
		yield return maxTime;
		callback.Invoke();
	}

	public void OurGames() {
#if UNITY_ANDROID
		Application.OpenURL("https://play.google.com/store/apps/dev?id=7220292605194330413");
#elif UNITY_IOS
        Application.OpenURL("https://apps.apple.com/us/developer/8floor/id500140554");
#endif
	}
}
