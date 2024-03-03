using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadVisualizer : MonoBehaviour {
    public Dictionary<string, RoadMesh> Roads = new Dictionary<string, RoadMesh>();
    public int Accuracy = 4;
    public RoadMesh LastRoadMesh;
    string nameContainer;
    ElementRoadLayerView layer;

    public void Init(ElementRoadLayerView layer) {
        this.nameContainer = layer.gameObject.name;
        this.layer = layer;
    }

    public void SetActive(bool value) {
        GetParentRoads().SetActive(value);
    }

    public IEnumerator LoadResources() {
        if(layer.RoadContainer == null) yield break;
        foreach(string id in layer.RoadContainer.Roads.Keys) {
            yield return ModuleContainer.I.SpriteController.LoadSprite(layer.RoadContainer.Roads[id].Path, false);
        }
    }

    public void Draw() {
        foreach(string id in layer.RoadContainer.Roads.Keys) {
            if(!Roads.ContainsKey(id)) {
                RoadMesh road = RoadMesh.CreateRoad(layer.RoadContainer.Roads[id], Accuracy);
                if (road == null) continue;

                Roads.Add(id, road);
                Roads[id].transform.SetParent(GetParentRoads().transform);
                Roads[id].transform.localPosition = layer.RoadContainer.Roads[id].Position;
                Roads[id].transform.GetComponent<RoadMesh>().ID = id;
                Roads[id].GetComponent<MeshRenderer>().sortingOrder = layer.LayerInOrder;
            }else {
                Roads[id].Points = layer.RoadContainer.Roads[id].Points;
                Roads[id].Finalized = false;
                Roads[id].GetComponent<MeshRenderer>().sortingOrder = layer.LayerInOrder;
                Roads[id].UpdateRoad();
                Roads[id].transform.localPosition = layer.RoadContainer.Roads[id].Position;
            }
        }
    }

    public void AddPointToLastRoad(Vector2 point) {
        if(LastRoadMesh == null) {
            LastRoadMesh = RoadMesh.CreateRoad(Accuracy, ModuleContainer.I.ViewsController.PanelRoadToolsView.SizeBrush, ModuleContainer.I.ViewsController.PanelRoadToolsView.PathCurrentRoad);
            LastRoadMesh.GetComponent<MeshRenderer>().sortingOrder = layer.LayerInOrder;
            LastRoadMesh.transform.SetParent(GetParentRoads().transform, false);
        }
        LastRoadMesh.AddPoint(point);
    }

    public void FinalizeLastRoad() {
        if(LastRoadMesh != null) {
            LastRoadMesh.FinalizeRoad();

            if(!string.IsNullOrEmpty(LastRoadMesh.ID)) Roads.Add(LastRoadMesh.ID, LastRoadMesh);
        }
    }

    public string GetIDLastRoad() {
        if(LastRoadMesh != null) return LastRoadMesh.ID;
        return string.Empty;
    }

    public Road GetLastRoad() {
        if(LastRoadMesh != null) return new Road(LastRoadMesh.Points, LastRoadMesh.Path, LastRoadMesh.Width, LastRoadMesh.transform.localPosition);
        return null;
    }

    public List<Vector2> GetPointsLastRoad() {
        if(LastRoadMesh != null) return LastRoadMesh.Points;
        return null;
    }

    public void ClearLastRoad() {
        if(LastRoadMesh != null) LastRoadMesh.FinalizeRoad();
        LastRoadMesh = null;
    }

    GameObject GetParentRoads() {
        if(transform.Find(string.Format("Roads_{0}", nameContainer))) {
            return transform.Find(string.Format("Roads_{0}", nameContainer)).gameObject;
        }
        GameObject result = new GameObject(string.Format("Roads_{0}", nameContainer));
        result.transform.SetParent(transform, false);
        result.transform.localPosition = Vector3.zero;
        return result;
    }
}
