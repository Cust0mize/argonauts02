using UnityEngine;

[System.Serializable]
public class PathPoint {
	public string ID;
	public Vector2 Position;

	public PathPoint(Vector2 position, string id) {
		Position = position;
		ID = id;
	}
}
