using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PathGraph {
	public const float POINT_SIZE = 17.06F;

	public List<PathPoint> Points = new List<PathPoint>();
	public List<PathJoint> Joints = new List<PathJoint>();

	public PathGraph () { }

	public PathGraph (List<PathPoint> points , List<PathJoint> joints) {
		Points = points;
		Joints = joints;
	}

	public void Add (Vector2 point) {
		if (!PointExist(point)) {
			Points.Add(new PathPoint(point , Guid.NewGuid().ToString()));
		}
	}

	//return id removed point
	public PathPoint Remove (Vector2 point) {
		PathPoint p = GetPoint(point);
		if (p != null) Points.Remove(p);
		return p;
	}

	public void RemoveJoints (PathJoint[] joints) {
		int countJoints = joints.Length;
		for (int i = 0;i < countJoints;i++) {
			Joints.Remove(joints[i]);
		}
	}

	public void RemoveJoint (string id) {
		int countJoints = Joints.Count;
		for (int i = 0;i < countJoints;i++) {
			if (Joints[i].ID.Equals(id)) {
				Joints.RemoveAt(i);
				break;
			}
		}
	}

	public void Join (Vector2 point1 , Vector2 point2) {
		PathPoint p1 = GetPoint(point1);
		PathPoint p2 = GetPoint(point2);

		if ((p1 != null && p2 != null) && (p1 != p2)) {
			if (!JointExist(p1 , p2)) {
				Joints.Add(new PathJoint(p1.ID , p2.ID , Guid.NewGuid().ToString()));
			}
		} else if (p1 != null && p2 == null) {
			p2 = new PathPoint(point2 , Guid.NewGuid().ToString());
			Points.Add(p2);

			if (!JointExist(p1 , p2)) {
				Joints.Add(new PathJoint(p1.ID , p2.ID , Guid.NewGuid().ToString()));
			}
		} else if (p1 == null && p2 != null) {
			p1 = new PathPoint(point1 , Guid.NewGuid().ToString());
			Points.Add(p1);

			if (!JointExist(p1 , p2)) {
				Joints.Add(new PathJoint(p1.ID , p2.ID , Guid.NewGuid().ToString()));
			}
		}
	}

	public void AddPointInJoint (string idJoint , Vector2 point) {
		PathPoint p = null;
		PathJoint j = null;

		if (!PointExist(point)) {
			p = new PathPoint(point , Guid.NewGuid().ToString());

			j = GetJoint(idJoint);

			PathPoint p1 = GetPoint(j.Point1ID);
			PathPoint p2 = GetPoint(j.Point2ID);

			RemoveJoint(idJoint);

			Points.Add(p);

			Join(p1.Position , p.Position);
			Join(p2.Position , p.Position);
		}
	}

	bool JointExist (PathPoint p1 , PathPoint p2) {
		return GetJoint(p1 , p2) != null ? true : false;
	}

	PathJoint GetJoint (PathPoint p1 , PathPoint p2) {
		int countJoints = Joints.Count;
		for (int i = 0;i < countJoints;i++) {
			if (Joints[i].Point1ID == p1.ID && Joints[i].Point2ID == p2.ID || Joints[i].Point1ID == p2.ID && Joints[i].Point2ID == p1.ID) return Joints[i];
		}
		return null;
	}

	PathJoint GetJoint (string id) {
		int countJoints = Joints.Count;
		for (int i = 0;i < countJoints;i++) {
			if (Joints[i].ID.Equals(id)) return Joints[i];
		}
		return null;
	}

	public PathJoint[] GetJoints (PathPoint p1) {
		List<PathJoint> result = new List<PathJoint>();

		int countJoints = Joints.Count;
		for (int i = 0;i < countJoints;i++) {
			if (Joints[i].Point1ID == p1.ID || Joints[i].Point2ID == p1.ID) result.Add(Joints[i]);
		}

		return result.ToArray();
	}

	bool PointExist (Vector2 point) {
		return GetPoint(point) != null ? true : false;
	}

	public PathPoint GetPoint (Vector2 point) {
		int countPoints = Points.Count;
		for (int i = 0;i < countPoints;i++) {
			if (point.x >= Points[i].Position.x - POINT_SIZE / 2 && point.x <= Points[i].Position.x + POINT_SIZE / 2 && point.y <= Points[i].Position.y + POINT_SIZE / 2 && point.y >= Points[i].Position.y - POINT_SIZE / 2) {
				return Points[i];
			}
		}
		return null;
	}

	public PathPoint GetPoint (string id) {
		int countPoints = Points.Count;
		for (int i = 0;i < countPoints;i++) {
			if (Points[i].ID.Equals(id)) {
				return Points[i];
			}
		}
		return null;
	}
}
