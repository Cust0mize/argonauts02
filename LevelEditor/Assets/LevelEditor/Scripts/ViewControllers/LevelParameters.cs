using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelParameters : MonoBehaviour {
	[Header ("Common")]

	[SerializeField]
	InputField levelTotalTimeInput;
	[SerializeField]
	InputField levelTime3Input;
	[SerializeField]
	InputField levelTime2Input;
	[SerializeField]
	InputField levelTime1Input;

	[Header ("Tasks")]

	[SerializeField]
	Dropdown task1KeyDropdown;
	[SerializeField]
	InputField task1ValueInput;

	[SerializeField]
	Dropdown task2KeyDropdown;
	[SerializeField]
	InputField task2ValueInput;

	[SerializeField]
	Dropdown task3KeyDropdown;
	[SerializeField]
	InputField task3ValueInput;

	[Header ("Max Value Upgrades")]
	[SerializeField]
	GameObject prefabUpgradeElement;
	[SerializeField]
	GameObject contentElements;

	[Header ("Start Resources")]
	[SerializeField]
	InputField foodValueInput;
	[SerializeField]
	InputField goldValueInput;
	[SerializeField]
	InputField stoneValueInput;
	[SerializeField]
	InputField woodValueInput;


	[Header ("Bonuses")]
	[SerializeField]
	Toggle resourceBonusToggle;
	[SerializeField]
	Toggle speedBonusToggle;
	[SerializeField]
	Toggle workerBonusToggle;
	[SerializeField]
	Toggle workBonusToggle;

	public bool Inited = false;

	public void InitDropdowns (List<string> options) {
		List<Dropdown.OptionData> templateOptions = new List<Dropdown.OptionData> ();
		templateOptions.Add (new Dropdown.OptionData (""));
		foreach (string o in options) {
			templateOptions.Add (new Dropdown.OptionData (o));
		}
			
		task1KeyDropdown.ClearOptions ();
		task2KeyDropdown.ClearOptions ();
		task3KeyDropdown.ClearOptions ();

		task1KeyDropdown.options = new List<Dropdown.OptionData> (templateOptions);
		task2KeyDropdown.options = new List<Dropdown.OptionData> (templateOptions);
		task3KeyDropdown.options = new List<Dropdown.OptionData> (templateOptions);
	}

	public void DoInit (Level level) {
		if (Inited)
			return;
		Inited = true;

		levelTotalTimeInput.text = level.TotalTime.ToString ();
		levelTime3Input.text = level.Time3.ToString ();
		levelTime2Input.text = level.Time2.ToString ();
		levelTime1Input.text = level.Time1.ToString ();

		if (level.Task1 != null) {
			for (int i = 0; i < task1KeyDropdown.options.Count; i++) {
				if (task1KeyDropdown.options [i].text.Equals (level.Task1.Key)) {
					task1KeyDropdown.value = i;
				}
			}
			task1ValueInput.text = level.Task1.Value.ToString ();
		} else {
			task1KeyDropdown.value = 0;
			task1ValueInput.text = string.Empty;
		}

		if (level.Task2 != null) {
			for (int i = 0; i < task2KeyDropdown.options.Count; i++) {
				if (task2KeyDropdown.options [i].text.Equals (level.Task2.Key)) {
					task2KeyDropdown.value = i;
				}
			}
			task2ValueInput.text = level.Task2.Value.ToString ();
		} else {
			task2KeyDropdown.value = 0;
			task2ValueInput.text = string.Empty;
		}

		if (level.Task3 != null) {
			for (int i = 0; i < task3KeyDropdown.options.Count; i++) {
				if (task3KeyDropdown.options [i].text.Equals (level.Task3.Key)) {
					task3KeyDropdown.value = i;
				}
			}
			task3ValueInput.text = level.Task3.Value.ToString ();
		} else {
			task3KeyDropdown.value = 0;
			task3ValueInput.text = string.Empty;
		}

		for (int i = 0; i < contentElements.transform.childCount - 1; i++) {
			Destroy (contentElements.transform.GetChild (i).gameObject);
		}
			
		if (level.MaxUpgradeValues == null || level.MaxUpgradeValues.Count == 0) {
			level.MaxUpgradeValues = new Dictionary<string, int> () {
				{ "tent", 0 },
				{ "quarry", 0 },
				{ "goldmine", 0 },
				{ "farm", 0 },
				{ "sawmill", 0 },
                { "blueportal", 0 },
                { "greenportal", 0 },
                { "yellowportal", 0 },
				{ "torch", 0 },
                { "port", 0 },
                { "oracletower", 0 },
                { "portalbig", 0 }
			};
		} else {
			Dictionary<string, int> templateUpgrades = new Dictionary<string, int> () {
				{ "tent", 0 },
				{ "quarry", 0 },
				{ "goldmine", 0 },
				{ "farm", 0 },
				{ "sawmill", 0 },
                { "blueportal", 0 },
                { "greenportal", 0 },
                { "yellowportal", 0 },
				{ "torch", 0 },
                { "port", 0 },
                { "oracletower", 0 },
                { "portalbig", 0 }
            };
			foreach (string u in templateUpgrades.Keys) {
				if (!level.MaxUpgradeValues.ContainsKey (u)) {
					level.MaxUpgradeValues.Add (u, 0);
				}
			}
		}

		foreach (string key in level.MaxUpgradeValues.Keys) {
			GameObject element = Instantiate (prefabUpgradeElement, contentElements.transform);
			element.GetComponent<UpgradeElement> ().Key = key;
			element.GetComponent<UpgradeElement> ().Value = level.MaxUpgradeValues [key];
			element.GetComponent<RectTransform> ().localPosition = new Vector3 (element.GetComponent<RectTransform> ().localPosition.x, element.GetComponent<RectTransform> ().localPosition.y, 0F);
			element.transform.SetSiblingIndex (contentElements.transform.childCount - 2);
		}

		foodValueInput.text = level.StartFood.ToString ();
		goldValueInput.text = level.StartGold.ToString ();
		stoneValueInput.text = level.StartStone.ToString ();
		woodValueInput.text = level.StartWood.ToString ();

		resourceBonusToggle.isOn = level.ResourceBonus;
		speedBonusToggle.isOn = level.SpeedBonus;
		workerBonusToggle.isOn = level.WorkerBonus;
		workBonusToggle.isOn = level.WorkBonus;
	}

	public float GetLevelTotalTime () {
		float time = 0f;
		float.TryParse (levelTotalTimeInput.text, out time);
		return time;
	}

	public float GetLevelTime3 () {
		float time = 0f;
		float.TryParse (levelTime3Input.text, out time);
		return time;
	}

	public float GetLevelTime2 () {
		float time = 0f;
		float.TryParse (levelTime2Input.text, out time);
		return time;
	}

	public float GetLevelTime1 () {
		float time = 0f;
		float.TryParse (levelTime1Input.text, out time);
		return time;
	}

	public Task GetTask1 () {
		Task task = new Task ();
		int value = 0;
		int.TryParse (task1ValueInput.text, out value);

		task.Key = task1KeyDropdown.options [task1KeyDropdown.value].text;
		task.Value = value;

		return task;
	}

	public Task GetTask2 () {
		Task task = new Task ();
		int value = 0;
		int.TryParse (task2ValueInput.text, out value);

		task.Key = task2KeyDropdown.options [task2KeyDropdown.value].text;
		task.Value = value;

		return task;
	}

	public Task GetTask3 () {
		Task task = new Task ();
		int value = 0;
		int.TryParse (task3ValueInput.text, out value);

		task.Key = task3KeyDropdown.options [task3KeyDropdown.value].text;
		task.Value = value;

		return task;
	}

	public Dictionary<string , int> GetMaxUpgradeValues () {
		Dictionary<string , int> result = new Dictionary<string , int> ();

		for (int i = 0; i < contentElements.transform.childCount - 1; i++) {
			result.Add (contentElements.transform.GetChild (i).GetComponent<UpgradeElement> ().Key, contentElements.transform.GetChild (i).GetComponent<UpgradeElement> ().Value);
		}

		return result;
	}

	public int GetFood () {
		int value = 0;
		int.TryParse (foodValueInput.text, out value); 
		return value;
	}

	public int GetGold () {
		int value = 0;
		int.TryParse (goldValueInput.text, out value); 
		return value;
	}

	public int GetStone () {
		int value = 0;
		int.TryParse (stoneValueInput.text, out value); 
		return value;
	}

	public int GetWood () {
		int value = 0;
		int.TryParse (woodValueInput.text, out value); 
		return value;
	}

	public bool GetResourceBonus () {
		return resourceBonusToggle.isOn;
	}

	public bool GetSpeedBonus () {
		return speedBonusToggle.isOn;
	}

	public bool GetWorkerBonus () {
		return workerBonusToggle.isOn;
	}

	public bool GetWorkBonus () {
		return workBonusToggle.isOn;
	}

	public void AddUpgrade () {
		GameObject element = Instantiate (prefabUpgradeElement, contentElements.transform);
		element.GetComponent<UpgradeElement> ().Key = string.Empty;
		element.GetComponent<UpgradeElement> ().Value = 0;
		element.GetComponent<RectTransform> ().localPosition = new Vector3 (element.GetComponent<RectTransform> ().localPosition.x, element.GetComponent<RectTransform> ().localPosition.y, 0F);
		element.transform.SetSiblingIndex (contentElements.transform.childCount - 2);
	}

	public void Close () {
		gameObject.SetActive (false);
	}
}
