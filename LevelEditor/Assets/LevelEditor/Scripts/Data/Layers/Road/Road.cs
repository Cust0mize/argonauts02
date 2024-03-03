using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Road {
    public List<Vector2> Points;
    public string Path;
    public float Width;
    public Vector2 Position;

    public Road() { Points = new List<Vector2>(); }
    public Road(List<Vector2> points, string path, float width, Vector2 position) {
        Points = points;
        Path = path;
        Width = width;
        Position = position;
    }
}
