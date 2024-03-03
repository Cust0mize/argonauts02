using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : BaseViewController {
	[SerializeField]
	LevelParameters levelParameters;

	public LevelParameters LevelParameters {
		get {
			return levelParameters;
		}
	}

	public override object GetModel () {
		Level level = new Level ();

		level.LayerContainer = ModuleContainer.I.ViewsController.PanelLayersView.GetModel () as LayerContainer;

		level.TotalTime = levelParameters.GetLevelTotalTime ();
		level.Time3 = levelParameters.GetLevelTime3 ();
		level.Time2 = levelParameters.GetLevelTime2 ();
		level.Time1 = levelParameters.GetLevelTime1 ();
		level.Task1 = levelParameters.GetTask1 ();
		level.Task2 = levelParameters.GetTask2 ();
		level.Task3 = levelParameters.GetTask3 ();
		level.StartFood = levelParameters.GetFood ();
		level.StartGold = levelParameters.GetGold ();
		level.StartStone = levelParameters.GetStone ();
		level.StartWood = levelParameters.GetWood ();
		level.MaxUpgradeValues = levelParameters.GetMaxUpgradeValues ();
		level.ResourceBonus = levelParameters.GetResourceBonus ();
		level.SpeedBonus = levelParameters.GetSpeedBonus ();
		level.WorkerBonus = levelParameters.GetWorkerBonus ();
		level.WorkBonus = levelParameters.GetWorkBonus ();
		return level;
	}

	public void OpenLevelParameters () {
		levelParameters.gameObject.SetActive (true);
		levelParameters.DoInit (model as Level);
	}

	public override string ToJson () {
		string result = string.Empty;
		Level model = GetModel () as Level;
		if (model != null) {
			result = JsonConvert.SerializeObject (model, ModuleContainer.I.ConfigController.GetJsonSettings ());
		}
		return result;
	}

	public void New () {
		model = new Level ();
		levelParameters.Inited = false;
		levelParameters.DoInit (model as Level);
	}

	public override void Init (object model = null) {
		GC.Collect ();

		if (model == null) {
			ModuleContainer.I.ViewsController.PanelLayersView.Init ();
			return;
		}

		Level level = model as Level;

		this.model = level;

		levelParameters.Inited = false;
		levelParameters.DoInit (model as Level);

		ModuleContainer.I.ViewsController.PanelLayersView.Init (level.LayerContainer);

		if (model == null)
			ModuleContainer.I.CameraController.GlobalReset ();
	}

	public override void FromJson (string json) {
		Level level = JsonConvert.DeserializeObject<Level> (json, ModuleContainer.I.ConfigController.GetJsonSettings ());

		Init (level);
	}
}
