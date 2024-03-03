using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSettingsView : BaseViewController {
    [SerializeField] InputField point1X, point1Y;
    [SerializeField] InputField point2X, point2Y;
    [SerializeField] InputField point3X, point3Y;
    [SerializeField] InputField point4X, point4Y;
    [SerializeField] InputField pivotX, pivotY;

    [SerializeField] InputField widthGrid, heightGrid;

    public override void Init(object model) {
        lockEdit = true;

        if((model as GridSettingsModel) == null) {
            throw new Exception("Set wrong type model");
        }

        this.model = model;

        GridSettingsModel gsm = model as GridSettingsModel;

        point1X.text = gsm.Point1.X.ToString();
        point2X.text = gsm.Point2.X.ToString();
        point3X.text = gsm.Point3.X.ToString();
        point4X.text = gsm.Point4.X.ToString();

        point1Y.text = gsm.Point1.Y.ToString();
        point2Y.text = gsm.Point2.Y.ToString();
        point3Y.text = gsm.Point3.Y.ToString();
        point4Y.text = gsm.Point4.Y.ToString();

        pivotX.text = gsm.PivotOffset.X.ToString();
        pivotY.text = gsm.PivotOffset.Y.ToString();

        widthGrid.text = gsm.GridSize.X.ToString();
        heightGrid.text = gsm.GridSize.Y.ToString();

        lockEdit = false;
    }

    public override void OnUpdateViewController() {
        if(lockEdit) return;

        if(model == null) {
            model = new GridSettingsModel();
        }

        GridSettingsModel gsm = model as GridSettingsModel;

        gsm.Point1.X = GetInt(point1X.text);
        gsm.Point2.X = GetInt(point2X.text);
        gsm.Point3.X = GetInt(point3X.text);
        gsm.Point4.X = GetInt(point4X.text);

        gsm.Point1.Y = GetInt(point1Y.text);
        gsm.Point2.Y = GetInt(point2Y.text);
        gsm.Point3.Y = GetInt(point3Y.text);
        gsm.Point4.Y = GetInt(point4Y.text);

        gsm.PivotOffset.X = GetInt(pivotX.text);
        gsm.PivotOffset.Y = GetInt(pivotY.text);

        gsm.GridSize.X = GetInt(widthGrid.text);
        gsm.GridSize.Y = GetInt(heightGrid.text);
    }
}
