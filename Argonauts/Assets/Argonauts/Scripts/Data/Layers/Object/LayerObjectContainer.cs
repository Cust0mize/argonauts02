using System.Collections.Generic;

[System.Serializable]
public class LayerObjectContainer {
	public Dictionary<string, LayerObject> Objects = new Dictionary<string, LayerObject>();

	public LayerObjectContainer() { }

	public LayerObjectContainer(Dictionary<string, LayerObject> objects) {
		Objects = objects;
	}

	public LayerObject RemoveObject(string id) {
		LayerObject obj = null;
		if (Objects.ContainsKey(id)) {
			obj = Objects[id];
			Objects.Remove(id);
		}
		return obj;
	}

	public LayerObject GetObject(string id) {
		if (Objects.ContainsKey(id)) return Objects[id];
		return null;
	}

	public void AddJoint(string objectID, string pointID) {
		if (Objects.ContainsKey(objectID)) {
			Objects[objectID].PointIDs.Add(pointID);
		}
	}

	public void RemoveJoint(string objectID, string pointID) {
		if (Objects.ContainsKey(objectID)) {
			for (int i = 0; i < Objects[objectID].PointIDs.Count; i++) {
				Objects[objectID].PointIDs.Remove(pointID);
			}
		}
	}
}