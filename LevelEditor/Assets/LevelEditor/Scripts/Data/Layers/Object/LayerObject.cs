using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LayerObject {
	public string Name;
	public List<string> PointIDs;
	public Vector2 Position;
	public string Path;
	public bool Flip = false;
	public List<string> SpecialPointsID;
	public List<string> JoinObjectsIDs;

	public LayerObject (string name, Vector2 position, string path, bool flip, List<string> pointsIDs = null, List<string> specialPointsID = null, List<string> joinObjectsIDs = null) {
		Name = name;
		Position = position;
		Path = path;
		PointIDs = pointsIDs;
		Flip = flip;
		SpecialPointsID = specialPointsID;
		JoinObjectsIDs = joinObjectsIDs;

		if (PointIDs == null) {
			PointIDs = new List<string> ();
		}
		if (SpecialPointsID == null) {
			SpecialPointsID = new List<string> ();
		}
		if (JoinObjectsIDs == null) {
			JoinObjectsIDs = new List<string> ();
		}
	}
}
