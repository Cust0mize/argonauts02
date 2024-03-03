using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoadSettingsView : BaseViewController {
    [SerializeField] InputField sizeBrush;

    public override void Init(object model) {
        lockEdit = true;

        if((model as RoadSettingsModel) == null) {
            throw new Exception("Set wrong type model");
        }

        this.model = model;

        RoadSettingsModel rsm = model as RoadSettingsModel;

        sizeBrush.text = rsm.SizeBrush.ToString();

        lockEdit = false;
    }

    public override void OnUpdateViewController() {
        if(lockEdit) return;

        if(model == null) {
            model = new RoadSettingsModel();
        }

        RoadSettingsModel rsm = model as RoadSettingsModel;

        rsm.SizeBrush = GetInt(sizeBrush.text);
    }
}
