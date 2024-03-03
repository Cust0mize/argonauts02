using System;
using System.Collections;
using System.Collections.Generic;
using HGL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelSettings : MonoBehaviour {
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider musicSlider;

    [SerializeField] private TextMeshProUGUI copyrightText;
    [SerializeField] private TextMeshProUGUI versionText;


    private void Awake() {
        copyrightText.text = ProjectSettings.I.copyright;
        versionText.text = LocalizationManager.GetLocalizedString("gui_app_version") + " " + ProjectSettings.AppVersion;

        soundSlider.onValueChanged.AddListener((float value) => {
            PlayerProfileManager.I.Settings.Values[SettingsData.SOUND_VOLUME_ID] = value;
            AudioManager.Instance.VolumeSounds = value;
        });
        musicSlider.onValueChanged.AddListener((float value) => {
            PlayerProfileManager.I.Settings.Values[SettingsData.MUSIC_VOLUME_ID] = value;
            AudioManager.Instance.VolumeMusic = value;
        });
    }

    private void OnEnable() {
        ApplySettings();
	}

    private void ApplySettings() {
        soundSlider.value = Convert.ToSingle(PlayerProfileManager.I.Settings.Values[SettingsData.SOUND_VOLUME_ID]);
        musicSlider.value = Convert.ToSingle(PlayerProfileManager.I.Settings.Values[SettingsData.MUSIC_VOLUME_ID]);
    }

	public void Ok() {
		PlayerProfileManager.I.SaveSettings();
		HGL_WindowManager.I.CloseWindow(null, null, "PanelSettings", false, false);
	}
}
