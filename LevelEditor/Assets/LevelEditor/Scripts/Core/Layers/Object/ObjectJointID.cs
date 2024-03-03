using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectJointID : ObjectID {
	LineRenderer lineRenderer;

	public LineRenderer LineRenderer {
		get {
			if (lineRenderer == null) {
				lineRenderer = GetComponent<LineRenderer> ();
			}
			return lineRenderer;
		}
	}

	[HideInInspector] public GameObject Point;
	[HideInInspector] public GameObject Object;

	public void Draw (float widthLine) {
		if (Point == null || Object == null) {
			Destroy (gameObject);
			return;
		}

		LineRenderer.positionCount = 2;
		LineRenderer.SetPosition (0, Point.transform.position);
		LineRenderer.SetPosition (1, GetTruePositionObject (Object.transform.position, Point.transform.position));
		LineRenderer.startWidth = widthLine;
		LineRenderer.endWidth = widthLine;
	}

	public void Remove () {
		Object.GetComponent<LayerObjectID> ().JointIDs.Remove (Point.GetComponent<ObjectID> ().ID);
		Destroy (gameObject);
	}

	Vector3 GetTruePositionObject (Vector3 objPos, Vector3 pointPos) {
		return new Vector3 (objPos.x, objPos.y, pointPos.z);
	}
}
