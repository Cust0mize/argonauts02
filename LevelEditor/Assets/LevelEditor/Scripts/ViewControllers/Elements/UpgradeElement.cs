using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeElement : MonoBehaviour {
	[SerializeField]
	InputField keyInput;
	[SerializeField]
	InputField valueInput;

	public string Key {
		get {
			return keyInput.text;
		}
		set {
			keyInput.text = value;
		}
	}
	public int Value {
		get {
			int result = 0;
			int.TryParse(valueInput.text , out result);
			return result;
		}
		set {
			valueInput.text = value.ToString();
		}
	}

	public void Remove () {
		DestroyImmediate(gameObject);
	}
}
