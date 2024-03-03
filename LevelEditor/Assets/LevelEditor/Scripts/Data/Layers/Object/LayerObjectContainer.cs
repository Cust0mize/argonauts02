using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class LayerObjectContainer {
	public Dictionary<string , LayerObject> Objects = new Dictionary<string , LayerObject> ();

	public LayerObjectContainer () {
	}

	public LayerObjectContainer (Dictionary<string , LayerObject> objects) {
		Objects = objects;
	}

	public LayerObjectInfo AddObject (Vector2 position, string spritePath) {
		string id = Guid.NewGuid ().ToString ();

		LayerObjectInfo result = new LayerObjectInfo (id, new LayerObject (Path.GetFileNameWithoutExtension (spritePath), position, spritePath, false));
		Objects.Add (id, result.LayerObject);
		return result;
	}

	public LayerObject RemoveObject (string id) {
		LayerObject obj = null;
		if (Objects.ContainsKey (id)) {
			obj = Objects [id];
			Objects.Remove (id);
		}
		return obj;
	}

    public bool PointJoined(string pointID) {
        foreach (string k in Objects.Keys) {
            if (Objects[k].PointIDs.Contains(pointID) || Objects[k].SpecialPointsID.Contains(pointID)) return true;
        }
        return false;
    }

	public void FlipObject (string id) {
		if (Objects.ContainsKey (id)) {
			Objects [id].Flip = !Objects [id].Flip;
		}
	}

	public LayerObject GetObject (string id) {
		if (Objects.ContainsKey (id))
			return Objects [id];
		return null;
	}

	public void ToggleSpecialPoint (string objectID, string pointID) {
		if (Objects.ContainsKey (objectID)) {
			if (!Objects [objectID].SpecialPointsID.Contains (pointID))
				Objects [objectID].SpecialPointsID.Add (pointID);
			else
				Objects [objectID].SpecialPointsID.Remove (pointID);
		}
	}

	public void AddJoint (string objectID, string pointID) {
		if (Objects.ContainsKey (objectID)) {
			if (!Objects [objectID].PointIDs.Contains (pointID))
				Objects [objectID].PointIDs.Add (pointID);
		}
	}

	public void AddJointObjects (string objectID1, string objectID2) {
		if (Objects.ContainsKey (objectID1) && Objects.ContainsKey (objectID2)) {
			if (!Objects [objectID1].JoinObjectsIDs.Contains (objectID2))
				Objects [objectID1].JoinObjectsIDs.Add (objectID2);
			if (!Objects [objectID2].JoinObjectsIDs.Contains (objectID1))
				Objects [objectID2].JoinObjectsIDs.Add (objectID1);
		}
	}

	public void RemoveJointObjects (string objectID1, string objectID2) {
		if (Objects.ContainsKey (objectID1)) {
			if (Objects [objectID1].JoinObjectsIDs.Contains (objectID2))
				Objects [objectID1].JoinObjectsIDs.Remove (objectID2);
		}
		if (Objects.ContainsKey (objectID2)) {
			if (Objects [objectID2].JoinObjectsIDs.Contains (objectID1))
				Objects [objectID2].JoinObjectsIDs.Remove (objectID1);
		}
	}

	public bool SpecialPointExistJoin (string pointID) {
		foreach (string k in Objects.Keys) {
			if (Objects [k].SpecialPointsID.Contains (pointID))
				return true;
		}
		return false;
	}

	public string GetObjectIDWithPoint (string pointID) {
		foreach (string k in Objects.Keys) {
			if (Objects [k].PointIDs.Contains (pointID))
				return k;
		}
		return string.Empty;
	}

	public void RemoveJoint (string objectID, string pointID) {
		if (Objects.ContainsKey (objectID)) {
			for (int i = 0; i < Objects [objectID].PointIDs.Count; i++) {
                if (SpecialPointExistJoin(pointID))
                    Objects[objectID].SpecialPointsID.Remove(pointID);
                Objects [objectID].PointIDs.Remove (pointID);
			}
		}
	}

	//public override int GetHashCode() {
	//    int result = 0;
	//    LayerObject[] objects = Objects.Values.ToArray();
	//    int countObjects = objects.Length;
	//    for(int i = 0; i < countObjects; i++) {
	//        result += objects[i].Position.GetHashCode() + objects[i].PointIDs.GetHashCode();
	//    }
	//    return result;
	//}
}