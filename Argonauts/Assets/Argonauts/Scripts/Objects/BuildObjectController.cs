using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildObjectController : MonoBehaviour {
    public object CustomData = null; //custom data for objects

	public bool Upgraded = false;
	public int UpgradeValue = 0;
	public BuildObject BaseBuildObject;
	public ResourcesDropPoint ResourcesDropPoint;
	public string DropPointID;
	public string ObjectKey;
	public bool Flip = false;

	private void Start() {
        ObjectKey = BaseBuildObject.BuildKey.Split('_')[2];
        ObjectKey = ObjectKey.Substring(0, ObjectKey.Length - 2);

		UpgradeValue = BaseBuildObject.Upgrade;

		BaseBuildObject.OnSuccessUpgrade += BaseBuildObject_OnSuccessUpgrade;
		BaseBuildObject.CanUpgrade = UpgradesManager.I.GetMaxUpgradeValue(ObjectKey.ToLower()) > UpgradeValue;
        BaseBuildObject.BuildObjectController = this;

		SpawnObject();
	}

	private void SpawnObject() {
		GameObject lastBuildObject = BaseBuildObject.gameObject;

		string fullkey = string.Format("{0}{1:00}", ObjectKey, UpgradeValue);

		BaseBuildObject = GameManager.I.SpawnObject(fullkey, BaseBuildObject).GetComponent<BuildObject>();
		BaseBuildObject.OnSuccessUpgrade += BaseBuildObject_OnSuccessUpgrade;
		BaseBuildObject.CanUpgrade = UpgradesManager.I.GetMaxUpgradeValue(ObjectKey.ToLower()) > UpgradeValue;
		BaseBuildObject.transform.localScale = new Vector3(Flip ? -1 : 1, 1, 1);
        BaseBuildObject.BuildObjectController = this;

        GameManager.I.AddObject(BaseBuildObject);

		if (BaseBuildObject.GetComponent<ProductionBuildObject>() != null) {
			if (ResourcesDropPoint == null) {
                ResourcesDropPoint = GameManager.I.GetObject(DropPointID).GetComponent<ResourcesDropPoint>();
			}

            ResourcesDropPoint.Node.IsFree = true;
			ResourcesDropPoint.TargetResources = BaseBuildObject.GetComponent<ProductionBuildObject>().DropResources;

			BaseBuildObject.GetComponent<ProductionBuildObject>().DropPoint = ResourcesDropPoint;
			BaseBuildObject.GetComponent<ProductionBuildObject>().StartWork();
		}

		DestroyImmediate(lastBuildObject.gameObject);
		DestroyImmediate(lastBuildObject);
	}

	void BaseBuildObject_OnSuccessUpgrade(int upgrade) {
		UpgradeValue = upgrade;
		BaseBuildObject.OnSuccessUpgrade -= BaseBuildObject_OnSuccessUpgrade;
		SpawnObject();
	}
}
