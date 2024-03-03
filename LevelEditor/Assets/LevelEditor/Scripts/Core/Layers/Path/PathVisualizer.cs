using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathVisualizer : MonoBehaviour {
	const float WIDTH_SIZE = 3F;

	public Dictionary<string , GameObject> PointsObjects = new Dictionary<string , GameObject> ();
	public Dictionary<string , GameObject> JointsObjects = new Dictionary<string , GameObject> ();

	ElementPathLayerView layer;

	public void Init (ElementPathLayerView layer) {
		this.layer = layer;
	}

	public void SetActive (bool value) {
		GetParentJoints ().SetActive (value);
		GetParentPoints ().SetActive (value);
	}

	public void Draw () {
		int countPoints = layer.PathGraph.Points.Count;
		int countJoints = layer.PathGraph.Joints.Count;

		for (int i = 0; i < countPoints; i++) {
			GameObject point = GetPoint (layer.PathGraph.Points [i].ID);

			if (point == null) {
				CreatePoint (layer.PathGraph.Points [i]);
			} else {
				point.transform.localPosition = layer.PathGraph.Points [i].Position;
				point.GetComponent<SpriteRenderer> ().sortingOrder = layer.LayerInOrder;
				point.GetComponent<SpriteRenderer> ().color = ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.LayerObjectContainer.SpecialPointExistJoin (layer.PathGraph.Points [i].ID) ? Color.yellow : (Color)new Color32 (64, 150, 234, 255);
			}
		}

		for (int i = 0; i < countJoints; i++) {
			GameObject joint = GetJoint (layer.PathGraph.Joints [i].ID);

			if (joint == null) {
				CreateJoint (GetPoint (layer.PathGraph.Joints [i].Point1ID).gameObject, GetPoint (layer.PathGraph.Joints [i].Point2ID).gameObject, layer.PathGraph.Joints [i].ID);
			} else {
				GameObject p1 = GetPoint (layer.PathGraph.Joints [i].Point1ID);
				GameObject p2 = GetPoint (layer.PathGraph.Joints [i].Point2ID);

				if (p1 != null && p2 != null) {
					joint.GetComponent<LineRenderer> ().SetPosition (0, p1.transform.position);
					joint.GetComponent<LineRenderer> ().SetPosition (1, p2.transform.position);
					joint.GetComponent<LineRenderer> ().sortingOrder = layer.LayerInOrder + 1;
					UpdateColliderLine (joint.GetComponent<LineRenderer> (), p1.transform.position, p2.transform.position);
				}
			}
		}
	}

	void CreatePoint (PathPoint point) {
		GameObject p = Instantiate (ModuleContainer.I.PrefabController.PathPointPrefab, GetParentPoints ().transform, false);
		p.GetComponent<SpriteRenderer> ().sortingOrder = layer.LayerInOrder;

		p.GetComponent<PointID> ().ID = point.ID;

		p.GetComponent<SpriteRenderer> ().color = ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.LayerObjectContainer.SpecialPointExistJoin (point.ID) ? Color.yellow : (Color)new Color32 (64, 150, 234, 255);

		p.transform.localPosition = point.Position;
		p.transform.localPosition = new Vector3 (p.transform.localPosition.x, p.transform.localPosition.y, 10);

		PointsObjects.Add (point.ID, p);
	}

	void CreateJoint (GameObject point1, GameObject point2, string id) {
		if (point1 == null || point2 == null)
			return;

		GameObject j = Instantiate (ModuleContainer.I.PrefabController.PathJointPrefab, GetParentJoints ().transform, false);

		j.GetComponent<ObjectID> ().ID = id;

		LineRenderer line = j.GetComponent<LineRenderer> ();
		line.positionCount = 2;
		line.SetPosition (0, point1.transform.position);
		line.SetPosition (1, point2.transform.position);
		line.startWidth = WIDTH_SIZE;
		line.endWidth = WIDTH_SIZE;
		line.sortingOrder = layer.LayerInOrder + 1;
		line.gameObject.layer = 12;

		AddColliderToLine (line, point1.transform.position, point2.transform.position);

		JointsObjects.Add (id, j);
	}

	private void AddColliderToLine (LineRenderer line, Vector2 startPoint, Vector2 endPoint) {
		BoxCollider lineCollider = new GameObject ("LineCollider").AddComponent<BoxCollider> ();

		lineCollider.gameObject.layer = 12;

		lineCollider.transform.parent = line.transform;

		float lineWidth = line.endWidth;

		float lineLength = Vector2.Distance (startPoint, endPoint);

		lineCollider.size = new Vector3 (lineLength - 15F, lineWidth * 3, 1f);

		Vector2 midPoint = (startPoint + endPoint) / 2;

		lineCollider.transform.position = midPoint;

		float angle = Mathf.Atan2 ((endPoint.y - startPoint.y), (endPoint.x - startPoint.x));

		angle *= Mathf.Rad2Deg;

		lineCollider.transform.Rotate (0, 0, angle);
	}

	private void UpdateColliderLine (LineRenderer line, Vector2 startPoint, Vector2 endPoint) {
		BoxCollider lineCollider = line.transform.GetChild (0).GetComponent<BoxCollider> ();

		if (lineCollider == null)
			return;

		lineCollider.transform.parent = line.transform;

		float lineWidth = line.endWidth;

		float lineLength = Vector2.Distance (startPoint, endPoint);

		lineCollider.size = new Vector3 (lineLength - 15F, lineWidth * 3, 1f);

		Vector2 midPoint = (startPoint + endPoint) / 2;

		lineCollider.transform.position = midPoint;

		float angle = Mathf.Atan2 ((endPoint.y - startPoint.y), (endPoint.x - startPoint.x));

		angle *= Mathf.Rad2Deg;

		lineCollider.transform.eulerAngles = Vector3.zero;

		lineCollider.transform.Rotate (0, 0, angle);
	}

	public void RemovePoint (string id) {
		GameObject p = GetPoint (id);
		if (p == null)
			return;
		if (PointsObjects.ContainsKey (id))
			PointsObjects.Remove (id);
		Destroy (p);
	}

	public void RemoveJoints (string[] id) {
		foreach (string i in id) {
			RemoveJoint (i);
		}
	}

	public void RemoveJoint (string id) {
		GameObject j = GetJoint (id);
		if (j == null)
			return;
		if (JointsObjects.ContainsKey (id))
			PointsObjects.Remove (id);
		Destroy (j);
	}

	public void ClearVisualizer () {
		foreach (string id in PointsObjects.Keys) {
			Destroy (PointsObjects [id].gameObject);
		}

		foreach (string id in JointsObjects.Keys) {
			Destroy (JointsObjects [id].gameObject);
		}

		PointsObjects.Clear ();
		JointsObjects.Clear ();
	}

	bool PointExist (string id) {
		return GetPoint (id) != null ? true : false;
	}

	bool JointExist (string id) {
		return GetJoint (id) != null ? true : false;
	}

	public GameObject GetPoint (string id) {
		if (PointsObjects.ContainsKey (id))
			return PointsObjects [id];
		return null;
	}

	GameObject GetJoint (string id) {
		if (JointsObjects.ContainsKey (id))
			return JointsObjects [id];
		return null;
	}

	GameObject GetParentPoints () {
		if (transform.Find ("PathPoints")) {
			return transform.Find ("PathPoints").gameObject;
		}
		GameObject result = new GameObject ("PathPoints");
		result.transform.SetParent (transform, false);
		result.transform.localPosition = Vector3.zero;
		return result;
	}

	GameObject GetParentJoints () {
		if (transform.Find ("PathJoints")) {
			return transform.Find ("PathJoints").gameObject;
		}
		GameObject result = new GameObject ("PathJoints");
		result.transform.SetParent (transform, false);
		result.transform.localPosition = Vector3.zero;
		return result;
	}
}
