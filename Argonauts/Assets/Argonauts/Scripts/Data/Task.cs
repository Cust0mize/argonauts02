using Newtonsoft.Json;

[System.Serializable]
public class Task {
	public string Key;
	public int Value;

	public Task() { }

	[JsonConstructor]
	public Task(string key, int value) {
		Key = key;
		Value = value;
	}
}
