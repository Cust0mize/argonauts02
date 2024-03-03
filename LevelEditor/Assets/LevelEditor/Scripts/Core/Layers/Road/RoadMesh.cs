using System;
using System.Collections.Generic;
using UnityEngine;

public class RoadMesh : MonoBehaviour {
	public List<Vector2> Points = new List<Vector2> ();
	public float Width = 0.5f;
	public int Accuracy = 3;
	public string ID;
	public string Path;
	public bool Destroyed = false;
	public bool Finalized = false;

	MeshCollider meshCollider;

	MeshCollider MeshCollider {
		get {
			if (meshCollider == null) {
				meshCollider = gameObject.AddComponent<MeshCollider> ();
			}
			return meshCollider;
		}
	}

	public static RoadMesh CreateRoad (Road road, int accuracy) {
        if (!ModuleContainer.I.SpriteController.GetSprite(road.Path)) {
            Debug.LogWarningFormat("Road sprite unknow: {0}", road.Path);
            return null;
        }

		GameObject r = Instantiate (ModuleContainer.I.PrefabController.RoadPrefab);
		r.layer = 10;
		RoadMesh roadMesh = r.AddComponent<RoadMesh> ();
		roadMesh.Accuracy = accuracy;
		roadMesh.Width = road.Width;
		roadMesh.Points = road.Points;
		roadMesh.GetComponent<MeshRenderer> ().material.mainTexture = ModuleContainer.I.SpriteController.GetSprite (road.Path).texture;
		roadMesh.UpdateRoad ();
		roadMesh.transform.localPosition = road.Position;
		return roadMesh;
	}

	public static RoadMesh CreateRoad (int accuracy, float width, string path) {
        if (!ModuleContainer.I.SpriteController.GetSprite(path)) {
            Debug.LogWarningFormat("Road sprite unknow: {0}", path);
            return null;
        }

		GameObject r = Instantiate (ModuleContainer.I.PrefabController.RoadPrefab);
		r.layer = 10;
		RoadMesh roadMesh = r.AddComponent<RoadMesh> ();
		roadMesh.Accuracy = accuracy;
		roadMesh.Width = width;
		roadMesh.GetComponent<MeshRenderer> ().material.mainTexture = ModuleContainer.I.SpriteController.GetSprite (path).texture;
		roadMesh.Path = path;
		return roadMesh;
	}

	public void AddPoint (Vector2 point) {
		Points.Add (point);
		UpdateRoad ();
	}

	public void FinalizeRoad () {
		if (Finalized)
			return;

		if (Points.Count <= 1) {
			Destroyed = true;
			Destroy (gameObject);
		} else {
			Finalized = true;

			if (string.IsNullOrEmpty (ID)) {
				ID = Guid.NewGuid ().ToString ();
			}
				
			GC.Collect ();
		}
	}

	public void UpdateRoad () {
		if (Finalized)
			return;

		if (Points.Count > 0) {
			if (gameObject != null) {
				gameObject.transform.localPosition = Points [0];
			}
		}

		if (Points.Count <= 1)
			return;

		if (Accuracy <= 1)
			Accuracy = 2;

		//get smooth points from rough path
		List<Vector2> smoothPoints = GetSpline (CalculateOffsets (Points, Points [0]));
		List<Vector2> roughStartCap = new List<Vector2> ();
		List<Vector2> roughEndCap = new List<Vector2> ();
		List<Vector2> smoothStartCap;
		List<Vector2> smoothEndCap;

		List<CombineInstance> instances = new List<CombineInstance> ();
		Vector2 posCurrentPoint = Vector2.zero;
		Vector2 posNextPoint = Vector2.zero;
		Vector2 dirCurrentPoint = Vector2.zero;
		Vector2 dirNextPoint = Vector2.zero;
		Vector2 rightPointPos = Vector2.zero;
		Vector2 leftPointPos = Vector2.zero;

		Vector2 startPos = Vector2.zero;
		Vector2 endPos = Vector2.zero;
		Vector2 leftStartPos = Vector2.zero;
		Vector2 rightStartPos = Vector2.zero;
		Vector2 leftEndPos = Vector2.zero;
		Vector2 rightEndPos = Vector2.zero;

		float magnitude = 0F;
		float offset = 0;
		float tiling = 0;

		List<CombineInstance> triangles = new List<CombineInstance> ();
		List<CombineInstance> trianglesStartCap = new List<CombineInstance> ();
		List<CombineInstance> trianglesEndCap = new List<CombineInstance> ();
		CombineInstance currentTriangle;

		#region start cap

		posCurrentPoint = smoothPoints [0];
		dirCurrentPoint = NormalizedDirection (posCurrentPoint, smoothPoints [1]);

		rightPointPos = RightPosition (posCurrentPoint, dirCurrentPoint, Width / 2);
		leftPointPos = LeftPosition (posCurrentPoint, dirCurrentPoint, Width / 2);

		roughStartCap.Add (leftPointPos);
		roughStartCap.Add (LeftPosition (DownPosition (posCurrentPoint, dirCurrentPoint, Width / 2), dirCurrentPoint, Width / 4));
		roughStartCap.Add (RightPosition (DownPosition (posCurrentPoint, dirCurrentPoint, Width / 2), dirCurrentPoint, Width / 4));
		roughStartCap.Add (rightPointPos);

		smoothStartCap = GetSpline (roughStartCap);

		int countPointsStartCap = smoothStartCap.Count;

		float uvXStartCap = 1F;

		for (int i = 0; i < countPointsStartCap; i++) {
			if (i + 1 < countPointsStartCap) {
				currentTriangle = new CombineInstance ();
				currentTriangle.mesh = Triangle (smoothStartCap [i], posCurrentPoint, smoothStartCap [i + 1]);
				currentTriangle.mesh.uv = new Vector2[] {
					new Vector2 (0, uvXStartCap),
					new Vector2 (0.5F, uvXStartCap),
					new Vector2 (0, uvXStartCap - ((smoothStartCap [i] - smoothStartCap [i + 1]).magnitude / 100F))
				};

				uvXStartCap += ((smoothStartCap [i] - smoothStartCap [i + 1]).magnitude / 100F);

				trianglesEndCap.Add (currentTriangle);
			}
		}

		#endregion

		int countPoints = smoothPoints.Count;
		for (int i = 0; i < countPoints; i++) {
			triangles.Clear ();

			//calculating points
			posCurrentPoint = smoothPoints [i];

			if (i + 1 < countPoints) {
				dirCurrentPoint = NormalizedDirection (posCurrentPoint, smoothPoints [i + 1]);

				posNextPoint = smoothPoints [i + 1];

				magnitude = (posNextPoint - posCurrentPoint).magnitude;
				tiling += magnitude / 100F;

				if (i + 2 < countPoints)
					dirNextPoint = NormalizedDirection (posNextPoint, smoothPoints [i + 2]);
			}

			rightPointPos = RightPosition (posCurrentPoint, dirCurrentPoint, Width / 2);
			leftPointPos = LeftPosition (posCurrentPoint, dirCurrentPoint, Width / 2);

			//generation mesh
			if (i + 1 < countPoints) {
				currentTriangle = new CombineInstance ();
				currentTriangle.mesh = Triangle (leftPointPos, RightPosition (posNextPoint, dirNextPoint, Width / 2), rightPointPos);
				triangles.Add (currentTriangle);
				currentTriangle = new CombineInstance ();
				currentTriangle.mesh = Triangle (leftPointPos, LeftPosition (posNextPoint, dirNextPoint, Width / 2), RightPosition (posNextPoint, dirNextPoint, Width / 2));
				triangles.Add (currentTriangle);

				currentTriangle = new CombineInstance ();
				currentTriangle.mesh = new Mesh ();
				currentTriangle.mesh.CombineMeshes (triangles.ToArray (), true, false);
				currentTriangle.mesh.uv = new Vector2[] {
					new Vector2 (0, offset),
					new Vector2 (1, tiling),
					new Vector2 (1, offset),
					new Vector2 (0, offset),
					new Vector2 (0, tiling),
					new Vector2 (1, tiling)
				};

				instances.Add (currentTriangle);
			}

			offset = tiling;
		}

		#region end cap

		roughEndCap.Add (leftPointPos);
		roughEndCap.Add (LeftPosition (UpPosition (posCurrentPoint, dirCurrentPoint, Width / 2), dirCurrentPoint, Width / 4));
		roughEndCap.Add (RightPosition (UpPosition (posCurrentPoint, dirCurrentPoint, Width / 2), dirCurrentPoint, Width / 4));
		roughEndCap.Add (rightPointPos);

		smoothEndCap = GetSpline (roughEndCap);

		int countPointsEndCap = smoothEndCap.Count;

		float uvXEndCap = 0F;

		for (int i = 0; i < countPointsEndCap; i++) {
			if (i + 1 < countPointsEndCap) {
				currentTriangle = new CombineInstance ();
				currentTriangle.mesh = Triangle (posCurrentPoint, smoothEndCap [i], smoothEndCap [i + 1]);
				currentTriangle.mesh.uv = new Vector2[] {
					new Vector2 (0.5F, uvXEndCap),
					new Vector2 (0, uvXEndCap),
					new Vector2 (0, uvXEndCap + ((smoothStartCap [i] - smoothStartCap [i + 1]).magnitude / 100F))
				};

				uvXEndCap += ((smoothStartCap [i] - smoothStartCap [i + 1]).magnitude / 100F);

				trianglesEndCap.Add (currentTriangle);
			}
		}

		#endregion

		List<CombineInstance> road = new List<CombineInstance> ();
		road.AddRange (trianglesStartCap);
		road.AddRange (instances);
		road.AddRange (trianglesEndCap);

		var mesh = new Mesh ();
		mesh.CombineMeshes (road.ToArray (), true, false);
		MeshCollider.sharedMesh = mesh;
		GetComponent<MeshFilter> ().mesh = mesh;
	}

	Mesh Triangle (Vector3 vertex0, Vector3 vertex1, Vector3 vertex2) {
		var normal = Vector3.Cross ((vertex1 - vertex0), (vertex2 - vertex0)).normalized;
		var mesh = new Mesh {
			vertices = new[] { vertex0, vertex1, vertex2 },
			normals = new[] { normal, normal, normal },
			triangles = new[] { 0, 1, 2 }
		};
		return mesh;
	}

	Vector2 NormalizedDirection (Vector2 from, Vector2 to) {
		return (to - from) / (to - from).magnitude;
	}

	Vector2 Right (Vector2 direction) {
		return new Vector2 (direction.y, -direction.x);
	}

	Vector2 RightPosition (Vector2 position, Vector2 direction, float width = 1) {
		return position + Right (direction) * width;
	}

	Vector2 LeftPosition (Vector2 position, Vector2 direction, float width = 1) {
		return position + -Right (direction) * width;
	}

	Vector2 DownPosition (Vector2 position, Vector2 direction, float width = 1) {
		return position + -direction * width;
	}

	Vector2 UpPosition (Vector2 position, Vector2 direction, float width = 1) {
		return position + direction * width;
	}

	List<Vector2> GetSpline (List<Vector2> points) {
		List<Vector2> result = new List<Vector2> ();

		int countPoints = points.Count;

		float[] x = new float[countPoints];
		float[] y = new float[countPoints];

		for (int i = 0; i < countPoints; i++) {
			x [i] = points [i].x;
			y [i] = points [i].y;
		}

		float[] xs, ys;
		Spline.FitParametric (x, y, points.Count * Accuracy - 1, out xs, out ys);

		for (int i = 0; i < xs.Length; i++) {
			result.Add (new Vector2 (xs [i], ys [i]));
		}

		return result;
	}

	List<Vector2> CalculateOffsets (List<Vector2> points, Vector2 position) {
		List<Vector2> result = new List<Vector2> ();

		int countPoints = points.Count;
		for (int i = 0; i < countPoints; i++) {
			result.Add (points [i] - position);
		}

		return result;
	}

	void OnDestroy () {
		Destroyed = true;
	}
}
