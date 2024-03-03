using System;
using System.Collections;
using System.Collections.Generic;
using HGL;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelEnterName : MonoBehaviour {
	[SerializeField] TMP_InputField inputField;
	[SerializeField] Button createButton;
	[SerializeField] Button cancelButton;

	public event Action<string> OnEnterNameSuccess = delegate { };
	public event Action OnEnterNameClose = delegate { };

	bool inited = false;

    public void Init(bool isCancelEnabled = true) {
		inputField.text = string.Empty;
        cancelButton.gameObject.SetActive(isCancelEnabled);
        createButton.gameObject.SetActive(false);

		EventSystem.current.SetSelectedGameObject(gameObject);
		EventSystem.current.SetSelectedGameObject(inputField.gameObject);

		if (!inited)
			inputField.onValueChanged.AddListener(OnValueNameChange);

		inited = true;
	}

	private void OnValueNameChange(string val) {
		createButton.gameObject.SetActive(val.Length > 0);
		cancelButton.gameObject.SetActive(val.Length.Equals(0));
		//TODO!!! Sound key
	}

	public void Ok() {
		if (!string.IsNullOrEmpty(inputField.text)) {
			HGL_WindowManager.I.CloseWindow(null, OnEnterNameAction, "PanelEnterName", false);
		} else {
			PanelCommonOk panel = HGL_WindowManager.I.GetWindow("PanelCommonOk").GetComponent<PanelCommonOk>();
            panel.Init(LocalizationManager.GetLocalizedString("menu_lbl_player_name_empty"));
			HGL_WindowManager.I.OpenWindow(null, null, "PanelCommonOk", false, true);
		}
	}

	public void SetFocusOnInputField() {
		inputField.ActivateInputField();
	}

	private void OnEnterNameAction() {
		OnEnterNameSuccess.Invoke(inputField.text);
	}

	public void Close() {
		HGL_WindowManager.I.CloseWindow(null, OnEnterNameClose, "PanelEnterName", false);
	}
}
