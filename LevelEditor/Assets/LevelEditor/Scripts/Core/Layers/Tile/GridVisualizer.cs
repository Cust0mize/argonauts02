using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour {
    List<GameObject> spritesGOs = new List<GameObject>();
    string nameContainer;
    ElementTileLayerView layer;

    public void Init(ElementTileLayerView layer) {
        this.nameContainer = layer.gameObject.name;
        this.layer = layer;
    }

    public void SetActive(bool value) {
        GetParentSprites().SetActive(value);
    }

    public IEnumerator LoadAllSprites() {
        for(int x = 0; x < layer.Grid.Size.X; x++) {
            for(int y = 0; y < layer.Grid.Size.Y; y++) {
                if(!string.IsNullOrEmpty(layer.Grid.GetCell(x, y).SpritePath)) {
                    yield return ModuleContainer.I.SpriteController.LoadSprite(layer.Grid.GetCell(x, y).SpritePath);
                }
            }
        }
    }

    public void Draw() {
        for(int x = 0; x < layer.Grid.Size.X; x++) {
            for(int y = 0; y < layer.Grid.Size.Y; y++) {
                if(string.IsNullOrEmpty(layer.Grid.GetCell(x, y).SpritePath)) {
                    DeleteSprite(layer.Grid.GetCell(x, y).Position);
                } else {
                    AddSprite(layer.Grid.GetCell(x, y));
                }
            }
        }
    }

    void AddSprite(GridCell cell) {
        Vector2 position = cell.Position.GetVector2();

        Transform existSprite = SpriteExist(position);
        if(existSprite != null) {
            existSprite.GetComponent<SpriteRenderer>().sortingOrder = layer.LayerInOrder;

            if(existSprite.GetComponent<SpriteRenderer>().sprite != ModuleContainer.I.SpriteController.GetSprite(cell.SpritePath)) {
                existSprite.GetComponent<SpriteRenderer>().sprite = ModuleContainer.I.SpriteController.GetSprite(cell.SpritePath);
            }
            return;
        }

        GameObject s = new GameObject(string.Format("sprite_{0}", position));
        SpriteRenderer sr = s.AddComponent<SpriteRenderer>();
        s.transform.SetParent(GetParentSprites().transform, false);
        s.transform.localPosition = position;
        s.layer = 11;

        sr.sprite = ModuleContainer.I.SpriteController.GetSprite(cell.SpritePath);
        sr.GetComponent<SpriteRenderer>().sortingOrder = layer.LayerInOrder;

        spritesGOs.Add(s);

        SortSprites();
    }

    void DeleteSprite(Integer2 pos) {
        Vector2 position = pos.GetVector2();

        for(int i = 0; i < spritesGOs.Count; i++) {
            if(pos.GetVector2().Equals((Vector2)spritesGOs[i].transform.position)) {
                Destroy(spritesGOs[i]);
                spritesGOs.RemoveAt(i);
            }
        }

        SortSprites();
    }

    void SortSprites() {
        int countSprites = spritesGOs.Count;
        for(int i = 0; i < countSprites; i++) {
            if(spritesGOs[i] != null) {
                spritesGOs[i].gameObject.transform.localPosition = new Vector3(spritesGOs[i].transform.localPosition.x, spritesGOs[i].transform.localPosition.y, Convert.ToSingle(0.01 * spritesGOs[i].transform.localPosition.y + 0.001 * spritesGOs[i].transform.localPosition.x) - 5);
            }
        }
    }

    void DestroyAllSprites() {
        int countSprites = spritesGOs.Count;
        for(int i = 0; i < countSprites; i++) {
            Destroy(spritesGOs[i]);
        }
        spritesGOs.Clear();
    }

    Transform SpriteExist(Vector2 position) {
        return GetParentSprites().transform.Find(string.Format("sprite_{0}", position));
    }

    GameObject GetParentSprites() {
        if(transform.Find(string.Format("GridSprites_{0}", nameContainer))) {
            return transform.Find(string.Format("GridSprites_{0}", nameContainer)).gameObject;
        }
        GameObject result = new GameObject(string.Format("GridSprites_{0}", nameContainer));
        result.transform.SetParent(transform, false);
        result.transform.localPosition = Vector3.zero;
        return result;
    }
}
