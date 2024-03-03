using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LocalizationImage : MonoBehaviour {

	public string stringId;

	void Awake () {
		Refresh ();
	}

	public void Refresh () {
		Image image = GetComponent<Image> ();

		if (image != null) {
			image.sprite = LocalizationManager.GetSprite (stringId);
		}
	}
}
