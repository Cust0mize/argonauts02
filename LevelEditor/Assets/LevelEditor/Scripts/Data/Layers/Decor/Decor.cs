using System;
using UnityEngine;

[Serializable]
public class Decor {
    public Vector2 Position;
    public string Path;

    public Decor(Vector2 position, string path) {
        Position = position;
        Path = path;
    }
}
