using System;

[Serializable]
public class PathJoint {
    public string ID;
    public string Point1ID;
    public string Point2ID;

    public PathJoint(string point1ID, string point2ID, string id) {
        Point1ID = point1ID;
        Point2ID = point2ID;
        ID = id;
    }
}
