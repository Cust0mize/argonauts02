using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PrefabContainer {
    public int LastSelectedPrefab;
    public Prefab[] Prefabs;

    public PrefabContainer() { }

    public PrefabContainer(Prefab[] prefabs) {
        Prefabs = prefabs;
    }

    public PrefabContainer(Prefab[] prefabs, int lastSelectedPrefab) {
        Prefabs = prefabs;
        LastSelectedPrefab = lastSelectedPrefab;
    }
}
