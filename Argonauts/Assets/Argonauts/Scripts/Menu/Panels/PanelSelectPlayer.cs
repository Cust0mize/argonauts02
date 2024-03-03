using System.Collections;
using System.Collections.Generic;
using HGL;
using UnityEngine;

public class PanelSelectPlayer : MonoBehaviour {
	[SerializeField] private ProfileItem[] profileItems;

	private string requestRemovePlayer;

	private void Awake() {
		foreach (ProfileItem pi in profileItems) {
			pi.OnButtonAddClickEvent += OnAddProfileEvent;
			pi.OnButtonRemoveClickEvent += OnRemoveProfileEvent;
			pi.OnButtonProfileClickEvent += OnSelectProfileEvent;
		}
	}

	public void Init() {
		List<ProfileData> profiles = PlayerProfileManager.I.GetProfiles();
		ProfileData currentProfile = PlayerProfileManager.I.GetCurrentProfile();

		for (int i = 0; i < profileItems.Length; i++) {
			if (i >= profiles.Count) {
				profileItems[i].SetActiveHolder(false);
				profileItems[i].SetActiveButtonAdd(false);

				if (i - 1 < profiles.Count)
					profileItems[i].SetActiveButtonAdd(true);
			} else {
				profileItems[i].gameObject.SetActive(true);
				profileItems[i].SetActiveHolder(true);
				profileItems[i].Init(profiles[i].Name, false);
				profileItems[i].Profile = profiles[i];
				profileItems[i].SetActive(currentProfile == profiles[i]);
			}
		}
	}

	private void OnAddProfileEvent(ProfileItem profileItem) {
		PanelEnterName panel = HGL_WindowManager.I.GetWindow("PanelEnterName").GetComponent<PanelEnterName>();
		panel.OnEnterNameSuccess += OnEnterNameSuccess;
		panel.OnEnterNameClose += OnEnterNameClose;
		panel.Init();
		HGL_WindowManager.I.OpenWindow(null, null, "PanelEnterName", false, true);
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
			Init();
		}
	}

	private void OnRemoveProfileEvent(ProfileItem profileItem) {
		if (PlayerProfileManager.I.GetProfiles().Count > 1) {
			requestRemovePlayer = profileItem.Profile.Name;

			PanelYesNo panel = HGL_WindowManager.I.GetWindow("PanelYesNo").GetComponent<PanelYesNo>();
			panel.Init(LocalizationManager.GetLocalizedString("menu_lbl_remove_player_warning"));
			panel.OnResultClose += OnResultRemoveProfileClose;
			HGL_WindowManager.I.OpenWindow(null, null, "PanelYesNo", false, true);
		} else {
			PanelCommonOk panelOk = HGL_WindowManager.I.GetWindow("PanelCommonOk").GetComponent<PanelCommonOk>();
			panelOk.Init(LocalizationManager.GetLocalizedString("menu_lbl_cant_remove_last_player"));
			HGL_WindowManager.I.OpenWindow(null, null, "PanelCommonOk", false, true);
		}
	}

	private void OnResultRemoveProfileClose(bool result) {
		PanelYesNo panel = HGL_WindowManager.I.GetWindow("PanelYesNo").GetComponent<PanelYesNo>();
		panel.OnResultClose -= OnResultRemoveProfileClose;

		if (result) {
			PlayerProfileManager.I.DeleteProfile(requestRemovePlayer);
			List<ProfileData> profiles = PlayerProfileManager.I.GetProfiles();
			PlayerProfileManager.I.SetCurrentProfile(profiles[profiles.Count - 1].Name);
			PlayerProfileManager.I.SaveAll();
			Init();
		}

		requestRemovePlayer = string.Empty;
	}

	private void OnSelectProfileEvent(ProfileItem profileItem) {
		foreach (ProfileItem pi in profileItems) {
			pi.SetActive(pi == profileItem);
		}
		PlayerProfileManager.I.SetCurrentProfile(profileItem.Profile.Name);
		PlayerProfileManager.I.SaveAll();
	}

	public void Ok() {
		HGL_WindowManager.I.CloseWindow(null, null, "PanelSelectPlayer", false);
	}
}
