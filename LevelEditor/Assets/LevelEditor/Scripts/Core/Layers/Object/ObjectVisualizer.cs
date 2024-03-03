using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectVisualizer : MonoBehaviour {
	const float WIDTH_SIZE = 3F;

	public Dictionary<string , GameObject> Objects = new Dictionary<string , GameObject> ();
	public Dictionary<string , ObjectJointID> Joints = new Dictionary<string , ObjectJointID> ();
	string nameContainer;
	ElementObjectLayerView layer;

	int lastDrawedHashCode;

	public void Init (ElementObjectLayerView layer) {
		this.nameContainer = layer.gameObject.name;
		this.layer = layer;
	}

	public void SetActive (bool value) {
		GetParentSprites ().SetActive (value);
	}

	public void SetActiveJoints (bool value) {
		GetParentJoints ().SetActive (value);
	}

	public IEnumerator LoadAllSprites () {
		if (layer.LayerObjectContainer == null)
			yield break;
		foreach (string id in layer.LayerObjectContainer.Objects.Keys) {
			yield return ModuleContainer.I.SpriteController.LoadSprite (layer.LayerObjectContainer.Objects [id].Path, false);
		}
	}

	public void Draw () {
		foreach (string id in layer.LayerObjectContainer.Objects.Keys) {
			if (Objects.ContainsKey (id)) {
				Objects [id].transform.localPosition = layer.LayerObjectContainer.Objects [id].Position;
				Objects [id].GetComponent<SpriteRenderer> ().sortingOrder = layer.LayerInOrder;

				RedrawJoints (Objects [id].GetComponent<LayerObjectID> ().JointIDs);
			} else {
				Objects.Add (id, CreateObject (layer.LayerObjectContainer.Objects [id], id));

                JoinObjects(GetJoinPointsObject(layer.LayerObjectContainer.Objects[id]), Objects[id]);
			}

			foreach (string i in layer.LayerObjectContainer.Objects[id].JoinObjectsIDs) {
				StartCoroutine (DelayJoinObjects (id, i));
			}
		}
			
		SortSprites ();
	}

	private IEnumerator DelayJoinObjects (string objectID1, string objectID2) {
		yield return new WaitForSeconds (0.1F);

        GameObject j = GetJointForObjects (objectID1, objectID2);

		if (j != null) {
			Joints.Remove (j.GetComponent<ObjectJointID> ().ID);
			DestroyObject (j);
        }

		CreateJoint (Objects [objectID1], Objects [objectID2], Guid.NewGuid ().ToString (), Color.blue, true);
	}

	public void AddObject (LayerObject obj, string id) {
		if (!Objects.ContainsKey (id)) {
			Objects.Add (id, CreateObject (obj, id));
		}
	}

	GameObject CreateObject (LayerObject obj, string id) {
		GameObject s = new GameObject (string.Format ("ObjectSprite_{0}", obj.Position));
		s.transform.SetParent (GetParentSprites ().transform, false);
		s.transform.localPosition = obj.Position;
		s.transform.localScale = new Vector3 (obj.Flip ? -1 : 1, 1, 1);
		s.layer = 9;

		SpriteRenderer sr = s.AddComponent<SpriteRenderer> ();

		sr.sprite = ModuleContainer.I.SpriteController.GetSprite (obj.Path);
		sr.GetComponent<SpriteRenderer> ().sortingOrder = layer.LayerInOrder;

		LayerObjectID layerObjectID = s.AddComponent<LayerObjectID> ();
		layerObjectID.ID = id;

		int countJoinPoints = obj.PointIDs.Count;
		for (int i = 0; i < countJoinPoints; i++) {
            try {
                ModuleContainer.I.ViewsController.PanelLayersView.PathLayer.PathVisualizer.GetPoint(obj.PointIDs[i])
               .GetComponent<PointID>().
               JoinLayerObjectID = id;
            } catch {
                Debug.Log(obj.PointIDs[i]);
            }
		}

		s.AddComponent<PolygonCollider2D> ();

		SortSprite (s);

		return s;
	}

	public void RemoveAndDestroyObject (string id) {
		if (Objects.ContainsKey (id)) {
			DestroyObject (Objects [id]);
			Objects.Remove (id);
		}
	}

	public void RemoveObject (string id) {
		if (Objects.ContainsKey (id)) {
			Objects.Remove (id);
		}
	}

    public void RemoveJoint (string objectID1, string objectID2) {
        GameObject j = GetJointForObjects(objectID1, objectID2);

        Debug.Log(j);

        if (j != null) {
            Joints.Remove(j.GetComponent<ObjectJointID>().ID);
            DestroyObject(j);
        }
    }

	public void FlipObject (string id) {
		if (Objects.ContainsKey (id)) {
			Objects [id].transform.localScale = new Vector3 (layer.LayerObjectContainer.GetObject (id).Flip ? -1 : 1, 1, 1);
		}
	}

	ObjectID[] GetJoinPointsObject (LayerObject obj) {
		List<ObjectID> result = new List<ObjectID> ();
		foreach (string pointID in obj.PointIDs) {
			result.Add (ModuleContainer.I.ViewsController.PanelLayersView.PathLayer.PathVisualizer.GetPoint (pointID).GetComponent<ObjectID> ());
		}
		return result.ToArray ();
	}

	public GameObject GetJointForObjects (string objectID1, string objectID2) {
		foreach (string k in Joints.Keys) {
            if ((Joints [k].GetComponent<ObjectJointID> ().Point.GetComponent<ObjectID>().ID.Equals (objectID1) && Joints [k].GetComponent<ObjectJointID> ().Object.GetComponent<ObjectID>().ID.Equals (objectID2)) || 
                (Joints [k].GetComponent<ObjectJointID> ().Point.GetComponent<ObjectID>().ID.Equals (objectID2) && Joints [k].GetComponent<ObjectJointID> ().Object.GetComponent<ObjectID>().ID.Equals (objectID1)))
				return Joints [k].gameObject;
		}
		return null;
	}

	public void JoinObjects (ObjectID[] points, GameObject obj) {
		for (int i = 0; i < points.Length; i++) {
			CreateJoint (points [i].gameObject, obj, points [i].ID, Color.red, true);
		}
	}

	public void JoinObject (GameObject point, GameObject obj, string id) {
		CreateJoint (point, obj, id, Color.red, true);
	}

	void CreateJoint (GameObject point1, GameObject obj, string id, Color color, bool joinObject = false) {
		if (point1 == null || obj == null)
			return;
		if (Joints.ContainsKey (id))
			return;

		if (color == null)
			color = Color.red;

		GameObject j = Instantiate (ModuleContainer.I.PrefabController.ObjectJointPrefab, GetParentJoints ().transform, false);

		ObjectJointID objJointID = j.GetComponent<ObjectJointID> ();
		objJointID.ID = id;
		objJointID.Point = point1;
		objJointID.Object = obj;
		objJointID.LineRenderer.sortingOrder = joinObject ? ModuleContainer.I.ViewsController.PanelLayersView.PathLayer.LayerInOrder : layer.LayerInOrder;
		objJointID.LineRenderer.material.color = color;
		objJointID.Draw (WIDTH_SIZE);

		obj.GetComponent<LayerObjectID> ().JointIDs.Add (id);

		Joints.Add (id, objJointID);
	}

	public void RemoveJoint (string id) {
		if (Joints.ContainsKey (id)) {
			Joints [id].Remove ();
			Joints.Remove (id);
		}
	}

	public void RemoveJoints (string[] ids) {
		int countIds = ids.Length;
		for (int i = 0; i < countIds; i++) {
			RemoveJoint (ids [i]);
		}
	}

	public void RemoveJoints (List<string> ids) {
		RemoveJoints (ids.ToArray ());
	}

	public void RedrawJoint (string id) {
		if (Joints.ContainsKey (id)) {
			Joints [id].Draw (WIDTH_SIZE);
		}
	}

	public void RedrawJoints (string[] ids) {
		foreach (string id in ids) {
			RedrawJoint (id);
		}
	}

	public void RedrawJoints (List<string> ids) {
		RedrawJoints (ids.ToArray ());
	}

	public void SortSprites () {
		foreach (string id in Objects.Keys) {
			Objects [id].gameObject.transform.localPosition = new Vector3 (Objects [id].transform.localPosition.x, Objects [id].transform.localPosition.y, Convert.ToSingle (0.01 * Objects [id].transform.localPosition.y + 0.001 * Objects [id].transform.localPosition.x) - 5);
		}
	}

	public void SortSprite (GameObject sprite) {
		sprite.transform.localPosition = new Vector3 (sprite.transform.localPosition.x, sprite.transform.localPosition.y, Convert.ToSingle (0.01 * sprite.transform.localPosition.y + 0.001 * sprite.transform.localPosition.x) - 5);
	}

	void ClearVisualizer () {
		foreach (string id in Objects.Keys) {
			Destroy (Objects [id].gameObject);
		}
		Objects.Clear ();
	}

	GameObject GetParentSprites () {
		if (transform.Find (string.Format ("ObjectSprites_{0}", nameContainer))) {
			return transform.Find (string.Format ("ObjectSprites_{0}", nameContainer)).gameObject;
		}
		GameObject result = new GameObject (string.Format ("ObjectSprites_{0}", nameContainer));
		result.transform.SetParent (transform, false);
		result.transform.localPosition = Vector3.zero;
		return result;
	}

	GameObject GetParentJoints () {
		if (transform.Find (string.Format ("ObjectJoints_{0}", nameContainer))) {
			return transform.Find (string.Format ("ObjectJoints_{0}", nameContainer)).gameObject;
		}
		GameObject result = new GameObject (string.Format ("ObjectJoints_{0}", nameContainer));
		result.transform.SetParent (transform, false);
		result.transform.localPosition = Vector3.zero;
		return result;
	}
}