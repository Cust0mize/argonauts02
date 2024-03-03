using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Prefab {
    public Dictionary<string, string> PrefabOptions = new Dictionary<string, string>(44);

    public string Name;

    public Prefab(string name, Dictionary<string, string> prefabOptions) {
        Name = name;
        PrefabOptions = prefabOptions;
    }

    public string GetSpritePath(string key) {
        if(!PrefabOptions.ContainsKey(key)) return string.Empty;
        return PrefabOptions[key];
    }
}