using System;
using System.Collections.Generic;

[Serializable]
public class RoadContainer {
    public Dictionary<string, Road> Roads;
    
    public RoadContainer() { Roads = new Dictionary<string, Road>(); }
    public RoadContainer(Dictionary<string, Road> roads) {
        Roads = roads;
    }

    public Road GetRoad(string id) {
        if(Roads.ContainsKey(id)) return Roads[id];
        return null;
    }
}
