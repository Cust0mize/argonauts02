using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoint {
    public string ID;
    public Vector2 Position;

    public PathPoint(Vector2 position, string id) {
        Position = position;
        ID = id;
    }
}
