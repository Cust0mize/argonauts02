using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using TMPro;

public class InputFieldSymbolController : MonoBehaviour {
	[SerializeField] private TMP_InputField inputField;

	private void Start() {
		inputField.onValueChanged.AddListener(UpdateText);
	}

	private void UpdateText(string text) {
		StringBuilder newText = new StringBuilder();

		for (int i = 0; i < text.Length; i++) {
			if (char.IsLetterOrDigit(text[i]) || char.IsWhiteSpace(text[i])) {
				newText.Append(text[i]);
			}
		}

		inputField.text = newText.ToString();
	}
}
