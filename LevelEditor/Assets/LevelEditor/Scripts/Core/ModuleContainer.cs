using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ModuleContainer : LocalSingletonBehaviour<ModuleContainer> {
    public ViewsController ViewsController;
    public CameraController CameraController;
    public ConfigController ConfigController;
    public InputController InputController;
    public SpriteController SpriteController;
    public PrefabController PrefabController;
    public ExportController ExportController;

    void Start() {
        ViewsController.StartInit();
    }
}
