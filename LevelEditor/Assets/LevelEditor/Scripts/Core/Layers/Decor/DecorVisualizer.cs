using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DecorVisualizer : MonoBehaviour {
    Dictionary<string, GameObject> Decors = new Dictionary<string, GameObject>();
    string nameContainer;
    ElementDecorLayerView layer;

    public void Init(ElementDecorLayerView layer) {
        this.nameContainer = layer.gameObject.name;
        this.layer = layer;
    }

    public void SetActive(bool value) {
        GetParentSprites().SetActive(value);
    }

    public IEnumerator LoadAllSprites() {
        if(layer.DecorContainer == null) yield break;
        foreach(string id in layer.DecorContainer.Decors.Keys) {
            yield return ModuleContainer.I.SpriteController.LoadSprite(layer.DecorContainer.Decors[id].Path, false);
        }
    }

    public void Draw() {
        foreach(string id in layer.DecorContainer.Decors.Keys) {
            if(Decors.ContainsKey(id)) {
                Decors[id].transform.localPosition = layer.DecorContainer.Decors[id].Position;
                Decors[id].GetComponent<SpriteRenderer>().sortingOrder = layer.LayerInOrder;
            } else {
                Decors.Add(id, CreateDecor(layer.DecorContainer.Decors[id], id));
            }
        }
        SortSprites();
    }

    public void AddDecor(Decor decor, string id) {
        if(!Decors.ContainsKey(id)) {
            Decors.Add(id, CreateDecor(decor, id));
        }
    }

    GameObject CreateDecor(Decor decor, string id) {
        GameObject s = new GameObject(string.Format("DecorSprite_{0}", decor.Position));
        s.transform.SetParent(GetParentSprites().transform, false);
        s.transform.localPosition = decor.Position;
        s.layer = 8;

        SpriteRenderer sr = s.AddComponent<SpriteRenderer>();

        sr.sprite = ModuleContainer.I.SpriteController.GetSprite(decor.Path);
        sr.GetComponent<SpriteRenderer>().sortingOrder = layer.LayerInOrder;

        ObjectID objectID = s.AddComponent<ObjectID>();
        objectID.ID = id;

        s.AddComponent<PolygonCollider2D>();

        SortSprite(s);

        return s;
    }

    public void RemoveAndDestroyDecor(string id) {
        if(Decors.ContainsKey(id)) {
            DestroyObject(Decors[id]);
            Decors.Remove(id);
        }
    }

    public void RemoveDecor(string id) {
        if(Decors.ContainsKey(id)) {
            Decors.Remove(id);
        }
    }

    public void SortSprites() {
        foreach(string id in Decors.Keys) {
            Decors[id].gameObject.transform.localPosition = new Vector3(Decors[id].transform.localPosition.x, Decors[id].transform.localPosition.y, Convert.ToSingle(0.01 * Decors[id].transform.localPosition.y + 0.001 * Decors[id].transform.localPosition.x) - 5);
        }
    }

    public void SortSprite(GameObject sprite) {
        sprite.transform.localPosition = new Vector3(sprite.transform.localPosition.x, sprite.transform.localPosition.y, Convert.ToSingle(0.01 * sprite.transform.localPosition.y + 0.001 * sprite.transform.localPosition.x) - 5);
    }

    void ClearVisualizer() {
        foreach(string id in Decors.Keys) {
            Destroy(Decors[id].gameObject);
        }
        Decors.Clear();
    }

    GameObject GetParentSprites() {
        if(transform.Find(string.Format("DecorSprites_{0}", nameContainer))) {
            return transform.Find(string.Format("DecorSprites_{0}", nameContainer)).gameObject;
        }
        GameObject result = new GameObject(string.Format("DecorSprites_{0}", nameContainer));
        result.transform.SetParent(transform, false);
        result.transform.localPosition = Vector3.zero;
        return result;
    }
}
