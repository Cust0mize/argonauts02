using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileItem : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI profileNameText;
	[SerializeField] private Button addButton;
	[SerializeField] private GameObject imageSelected;
	[SerializeField] private GameObject imageUnselected;
	[SerializeField] private GameObject holder;

	[HideInInspector] public ProfileData Profile;

	public event Action<ProfileItem> OnButtonProfileClickEvent = delegate { };
	public event Action<ProfileItem> OnButtonRemoveClickEvent = delegate { };
	public event Action<ProfileItem> OnButtonAddClickEvent = delegate { };

	public void Init(string name, bool canAdd) {
		profileNameText.text = name;
		addButton.gameObject.SetActive(canAdd);
	}

	public void OnButtonProfileClick() {
		SetActive(true);
		OnButtonProfileClickEvent.Invoke(this);
	}

	public void OnButtonRemoveClick() {
		OnButtonRemoveClickEvent.Invoke(this);
	}

	public void OnButtonAddClick() {
		OnButtonAddClickEvent.Invoke(this);
	}

	public void SetActive(bool value) {
		imageSelected.gameObject.SetActive(value);
		imageUnselected.gameObject.SetActive(!value);
	}

	public void SetActiveHolder(bool value) {
		holder.gameObject.SetActive(value);
	}

	public void SetActiveButtonAdd(bool value) {
		addButton.gameObject.SetActive(value);
	}
}
