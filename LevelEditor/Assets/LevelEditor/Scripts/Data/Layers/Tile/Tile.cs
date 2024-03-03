using UnityEngine;

public class Tile {
    public string Path;
    public Sprite Sprite;

    public Tile() { }

    public Tile(string path, Sprite sprite) {
        Path = path;
        Sprite = sprite;
    }
}
