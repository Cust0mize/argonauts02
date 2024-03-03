using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PanelPrefabFillView : BaseViewController {
    [SerializeField] ButtonCellView[] cells;

    public override void StartInit() {
        foreach(ButtonCellView b in cells) {
            b.StartInit();
        }
    }

    public override void Show() {
        base.Show();
    }

    public override void Init(object model = null) {
        lockEdit = true;

        base.Init(model);

        if(model != null) {
            Prefab p = model as Prefab;

            string[] values = p.PrefabOptions.Values.ToArray();

            for(int i = 0; i < cells.Length; i++) {
                cells[i].Init();
            }

            for(int i = 0; i < values.Length; i++) {
                if(i < cells.Length) cells[i].Init(values[i]);
            }
        }

        lockEdit = false;
    }

    public void Save() {
        Dictionary<string, string> prefabOptions = new Dictionary<string, string>();

        AddTileOption(prefabOptions, "0000", cells[0].Path);
        AddTileOption(prefabOptions, "0001", cells[1].Path);
        AddTileOption(prefabOptions, "0101", cells[2].Path);
        AddTileOption(prefabOptions, "0100", cells[3].Path);
        AddTileOption(prefabOptions, "1000", cells[4].Path);
        AddTileOption(prefabOptions, "1010", cells[5].Path);
        AddTileOption(prefabOptions, "0010", cells[6].Path);
        AddTileOption(prefabOptions, "1001", cells[7].Path);
        AddTileOption(prefabOptions, "1101", cells[8].Path);
        AddTileOption(prefabOptions, "1100", cells[9].Path);
        AddTileOption(prefabOptions, "1011", cells[10].Path);
        AddTileOption(prefabOptions, "1111", cells[11].Path);
        AddTileOption(prefabOptions, "1110", cells[12].Path);
        AddTileOption(prefabOptions, "0011", cells[13].Path);
        AddTileOption(prefabOptions, "0111", cells[14].Path);
        AddTileOption(prefabOptions, "0110", cells[15].Path);

        ModuleContainer.I.SpriteController.AddSprites(prefabOptions.Values.ToArray());

        (ModuleContainer.I.ViewsController.PanelTilePrefabsView.SelectedPrefab.GetModel() as Prefab).PrefabOptions = prefabOptions;
    }

    void AddTileOption(Dictionary<string, string> dict, string key, string path) {
        if(dict.ContainsKey(key)) {
            dict[key] = path;
            return;
        }
        dict.Add(key, path);
    }
}
