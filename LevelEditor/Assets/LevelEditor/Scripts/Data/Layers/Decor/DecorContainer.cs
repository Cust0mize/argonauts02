using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DecorContainer {
    public Dictionary<string, Decor> Decors = new Dictionary<string, Decor>();

    public DecorContainer() { }

    public DecorContainer(Dictionary<string, Decor> decors) {
        Decors = decors;
    }

    public DecorInfo AddDecor(Vector2 position, string spritePath) {
        DecorInfo result = new DecorInfo(new Decor(position, spritePath), Guid.NewGuid().ToString());
        Decors.Add(result.ID, result.Decor);
        return result;
    }

    public Decor RemoveDecor(string id) {
        Decor decor = null;
        if(Decors.ContainsKey(id)) {
            decor = Decors[id];
            Decors.Remove(id);
        }
        return decor;
    }

    public Decor GetDecor(string id) {
        if(Decors.ContainsKey(id)) return Decors[id];
        return null;
    }
}

public class DecorInfo {
    public Decor Decor;
    public string ID;

    public DecorInfo() { }
    public DecorInfo(Decor decor, string id) {
        Decor = decor;
        ID = id;
    }
}
