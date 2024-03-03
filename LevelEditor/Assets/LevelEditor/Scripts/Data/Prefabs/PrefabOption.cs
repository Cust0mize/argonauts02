using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class PrefabOption {
    public class TileOption {
        public string SpritePath;
        public Integer2 Offset;

        public TileOption(string spritePath, Integer2 offset) {
            SpritePath = spritePath;
            Offset = offset;
        }
    }

    public string[] Key;
    public TileOption[] Tiles;

    public PrefabOption(string[] key, TileOption[] tiles) {
        Key = key;
        Tiles = tiles;
    }
}
