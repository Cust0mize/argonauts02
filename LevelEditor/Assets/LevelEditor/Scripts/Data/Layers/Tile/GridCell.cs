using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GridCell {
    public int X, Y;
    public Integer2 Position;
    public string SpritePath = string.Empty;

    [NonSerialized] public GridCell LEFT;
    [NonSerialized] public GridCell RIGHT;
    [NonSerialized] public GridCell UP;
    [NonSerialized] public GridCell DOWN;

    public GridCell(string spritePath, Integer2 position, int x, int y) {
        SpritePath = spritePath;
        Position = position;
        X = x;
        Y = y;
    }

    public GridCell[] GetNeighbors() {
        List<GridCell> result = new List<GridCell>();

        result.Add(DOWN);
        result.Add(LEFT);
        result.Add(UP);
        result.Add(RIGHT);

        return result.ToArray();
    }

    public int CountFillNeighbors() {
        int count = 0;

        if(DOWN != null && !string.IsNullOrEmpty(DOWN.SpritePath)) count++;
        if(LEFT != null && !string.IsNullOrEmpty(LEFT.SpritePath)) count++;
        if(UP != null && !string.IsNullOrEmpty(UP.SpritePath)) count++;
        if(RIGHT != null && !string.IsNullOrEmpty(RIGHT.SpritePath)) count++;

        return count;
    }

    void PingNeighbors(GridCell excludeNeighbour = null) {
        GridCell[] neighbors = GetNeighbors();
        int countNeighbors = neighbors.Length;

        for(int i = 0; i < countNeighbors; i++) {
            if(neighbors[i] == null) continue;
            if(string.IsNullOrEmpty(neighbors[i].SpritePath)) continue;

            if(neighbors[i] != null && neighbors[i] != excludeNeighbour) {
                if(excludeNeighbour==null) {
                    neighbors[i].UpdateSprite(this);
                }else {
                    neighbors[i].UpdateSprite(excludeNeighbour);
                }
            }
        }
    }
     
    public void UpdateSprite(GridCell excludeNeighbour) {
        string oldPath = SpritePath;
        SpritePath = (ModuleContainer.I.ViewsController.PanelTilePrefabsView.SelectedPrefab.GetModel() as Prefab).GetSpritePath(GetPrefabKey().ToString("D4"));


        if(!oldPath.Equals(SpritePath)) {
            PingNeighbors(excludeNeighbour);
        }
    }

    public void SetSprite(string spritePath, bool hardDraw = true) {
        bool needPing = SpritePath != spritePath ? true : false;

        SpritePath = spritePath;

        if(!hardDraw && needPing) {
            if(string.IsNullOrEmpty(spritePath)) {
                SpritePath = "";
            } else {
                SpritePath = (ModuleContainer.I.ViewsController.PanelTilePrefabsView.SelectedPrefab.GetModel() as Prefab).GetSpritePath(GetPrefabKey().ToString("D4"));
            }

            PingNeighbors();
        }
    }

    public int GetPrefabKey() {
        int prefabKey = 0;
       
        if(DOWN != null && !string.IsNullOrEmpty(DOWN.SpritePath)) prefabKey += 1000;
        if(LEFT != null && !string.IsNullOrEmpty(LEFT.SpritePath)) prefabKey += 100;
        if(UP != null && !string.IsNullOrEmpty(UP.SpritePath)) prefabKey += 10;
        if(RIGHT != null && !string.IsNullOrEmpty(RIGHT.SpritePath)) prefabKey += 1;

        return prefabKey;
    }

    public bool CanUse(Integer2 point) {
        return BelongQuadrangle(new Integer2[4] { Position, GetPositionOffset(Position, 1), GetPositionOffset(Position, 2), GetPositionOffset(Position, 3) }, point);
    }

    Integer2 GetPositionOffset(Integer2 position, int number) {
        switch(number) {
            case 0:
                return position;
            case 1:
                return position + (ModuleContainer.I.ViewsController.GridSettingsView.GetModel() as GridSettingsModel).Point2;
            case 2:
                return position + (ModuleContainer.I.ViewsController.GridSettingsView.GetModel() as GridSettingsModel).Point3;
            case 3:
                return position + (ModuleContainer.I.ViewsController.GridSettingsView.GetModel() as GridSettingsModel).Point4;
        }
        return Integer2.Zero;
    }

    bool BelongQuadrangle(Integer2[] quadrangle, Integer2 point) {
        Integer2[] triangle1 = new Integer2[3] { quadrangle[1], quadrangle[2], quadrangle[3] };
        Integer2[] triangle2 = new Integer2[3] { quadrangle[1], quadrangle[0], quadrangle[3] };

        return BelongTriangle(triangle1, point) || BelongTriangle(triangle2, point);
    }

    bool BelongTriangle(Integer2[] triangle, Integer2 point) {
        float[] results = new float[3];

        results[0] = (triangle[0].X - point.X) * (triangle[1].Y - triangle[0].Y) - (triangle[1].X - triangle[0].X) * (triangle[0].Y - point.Y);
        results[1] = (triangle[1].X - point.X) * (triangle[2].Y - triangle[1].Y) - (triangle[2].X - triangle[1].X) * (triangle[1].Y - point.Y);
        results[2] = (triangle[2].X - point.X) * (triangle[0].Y - triangle[2].Y) - (triangle[0].X - triangle[2].X) * (triangle[2].Y - point.Y);

        if(ZeroExist(results)) {
            return false;
        } else if(SameSing(results)) {
            return true;
        } else {
            return false;
        }
    }

    bool ZeroExist(float[] numbers) {
        foreach(float n in numbers) {
            if(Mathf.Approximately(n, 0F)) {
                return true;
            }
        }
        return false;
    }

    bool SameSing(float[] numbers) {
        bool znak = numbers[0] >= 0 ? true : false;

        foreach(float n in numbers) {
            if(!znak && n >= 0) {
                return false;
            } else if(znak && n < 0) {
                return false;
            }
        }
        return true;
    }
}