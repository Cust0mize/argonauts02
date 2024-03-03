using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCellsVisualizer : MonoBehaviour {
    [SerializeField] Material material;
    [SerializeField] float widthGridLines = 0.75F;

    List<GameObject> lineRenderers = new List<GameObject>();

    GameObject parent;
    GameObject Parent {
        get {
            if(parent == null) {
                parent = GetParentLines();
            }
            return parent;
        }
    }
    string nameContainer;

    ElementTileLayerView layer;
    Grid Grid {
        get {
            return (layer.GetModel() as TileLayer).Grid;
        }
    }

    public void Init(ElementTileLayerView layer) {
        this.nameContainer = layer.gameObject.name;
        this.layer = layer;
    }

    public void ReDraw() {
        bool active = false;
        if(lineRenderers.Count> 0) {
            active = lineRenderers[0].gameObject.activeSelf;
        }
        ClearAllLines();
        Draw(false);

        SetActiveLines(active);
    }

    public void Draw(bool justActive = true) {
        SetActiveLines(true);

        if(justActive) {
            if(Parent.transform.childCount > 0) {
                return;
            }
        }

        Integer2 offset = Integer2.Zero;

        //offset.X = (ModuleContainer.I.ViewsController.GridSettingsView.GetModel() as GridSettingsModel).GetWidth();
        //offset.Y = -(ModuleContainer.I.ViewsController.GridSettingsView.GetModel() as GridSettingsModel).GeTrueHeight();

        for(int x = 0; x < Grid.Size.X; x++) {
            AddLine(string.Format("line_x_{0}", x), GetOffset(Grid.GetCell(x, 0).Position, 1) + offset, GetOffset(Grid.GetCell(x, Grid.Size.Y - 1).Position, 2) + offset);

            if(x + 1 == Grid.Size.X) {
                AddLine(string.Format("line_x_{0}", x), GetOffset(Grid.GetCell(x, 0).Position, 0) + offset, GetOffset(Grid.GetCell(x, Grid.Size.Y - 1).Position, 3) + offset);
            }
        }

        for(int y = 0; y < Grid.Size.Y; y++) {
            AddLine(string.Format("line_y_{0}", y), GetOffset(Grid.GetCell(Grid.Size.X - 1, y).Position, 0) + offset, GetOffset(Grid.GetCell(0, y).Position, 1) + offset);

            if(y + 1 == Grid.Size.Y) {
                AddLine(string.Format("line_y_{0}", y), GetOffset(Grid.GetCell(Grid.Size.X - 1, y).Position, 3) + offset, GetOffset(Grid.GetCell(0, Grid.Size.Y - 1).Position, 2) + offset);
            }
        }
    }

    Integer2 GetOffset(Integer2 position, int number) {
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

    Integer2 GetInteger2(Vector2 value) {
        return new Integer2((int)value.x, (int)value.y);
    }

    void AddLine(string postfix, Integer2 a, Integer2 b) {
        GameObject g = new GameObject(string.Format("line_renderer_{0}", postfix));
        LineRenderer lineRenderer = g.AddComponent<LineRenderer>();
        lineRenderer.startWidth = widthGridLines;
        lineRenderer.endWidth = widthGridLines;
        lineRenderer.material = material;
        lineRenderer.useWorldSpace = false;

        lineRenderer.positionCount = 2;

        lineRenderer.SetPosition(0, a.GetVector2());
        lineRenderer.SetPosition(1, b.GetVector2());

        g.transform.SetParent(Parent.transform, false);

        lineRenderers.Add(g);
    }

    public void ClearAllLines() {
        int countLines = lineRenderers.Count;
        for(int i = 0; i < countLines; i++) {
            Destroy(lineRenderers[i].gameObject);
        }
        lineRenderers.Clear();
    }

    public void SetActiveLines(bool value) {
        int countLines = lineRenderers.Count;
        for(int i = 0; i < countLines; i++) {
            lineRenderers[i].SetActive(value);
        }
    }

    GameObject GetParentLines() {
        if(transform.Find(string.Format("GridLines_{0}", nameContainer))) {
            return transform.Find(string.Format("GridLines_{0}", nameContainer)).gameObject;
        }
        GameObject result = new GameObject(string.Format("GridLines_{0}", nameContainer));
        result.transform.SetParent(transform, false);
        result.transform.localPosition = Vector3.zero;
        return result;
    }
}
