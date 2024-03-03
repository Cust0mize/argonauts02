using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class Level {
	public LayerContainer LayerContainer;
	public float TotalTime;
	public float Time3;
	public float Time2;
	public float Time1;
	public Task Task1;
	public Task Task2;
	public Task Task3;
	public Dictionary<string , int> MaxUpgradeValues = new Dictionary<string , int> ();
	public int StartFood;
	public int StartGold;
	public int StartStone;
	public int StartWood;
	public bool ResourceBonus;
	public bool SpeedBonus;
	public bool WorkerBonus;
	public bool WorkBonus;

	public Level () {
		LayerContainer = new LayerContainer ();
	}

	public Level (LayerContainer layerContainer) {
		LayerContainer = layerContainer;
	}

	[JsonConstructor]
	public Level (
		LayerContainer layerContainer, float totalTime, float time3, float time2, float time1,
		Task task1, Task task2, Task task3,
		Dictionary<string , int> maxUpgradeValues,
		int startFood, int startGold, int startStone, int startWood,
		bool resourceBonus, bool speedBonus, bool workerBonus, bool workBonus
	) {
		LayerContainer = layerContainer;

		TotalTime = totalTime;
		Time3 = time3;
		Time2 = time2;
		Time1 = time1;

		Task1 = task1;
		Task2 = task2;
		Task3 = task3;

		MaxUpgradeValues = maxUpgradeValues;

		StartFood = startFood;
		StartGold = startGold;
		StartStone = startStone;
		StartWood = startWood;

		ResourceBonus = resourceBonus;
		SpeedBonus = speedBonus;
		WorkerBonus = workerBonus;
		WorkBonus = workBonus;
	}
}
