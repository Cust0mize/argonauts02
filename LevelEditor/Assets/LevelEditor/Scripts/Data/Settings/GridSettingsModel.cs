using System;
using UnityEngine;

[Serializable]
public class GridSettingsModel {
    public Integer2 Point1;
    public Integer2 Point2;
    public Integer2 Point3;
    public Integer2 Point4;
    public Integer2 PivotOffset;

    public Integer2 GridSize;

    public GridSettingsModel() {
        Point1 = Integer2.Zero;
        Point2 = Integer2.Zero;
        Point3 = Integer2.Zero;
        Point4 = Integer2.Zero;
        PivotOffset = Integer2.Zero;

        GridSize = Integer2.Zero;
    }

    public GridSettingsModel(Integer2 p1, Integer2 p2, Integer2 p3, Integer2 p4, Integer2 p, Integer2 gz) {
        Point1 = p1;
        Point2 = p2;
        Point3 = p3;
        Point4 = p4;
        PivotOffset = p;
        GridSize = gz;
    }

    public int GetWidth() {
        return Mathf.Abs(Point2.X);
    }

    public int GetHeight() {
        return Mathf.Abs(Point4.Y);
    }

    public int GetTrueHeight() {
        return Mathf.Abs(Point1.Y - Point2.Y);
    }

    public int GetFullWidth() {
        return Mathf.Abs(Point2.X - Point4.X);
    }

    public int GetFullHeight() {
        return Mathf.Abs(Point1.Y - Point3.Y);
    }
}
