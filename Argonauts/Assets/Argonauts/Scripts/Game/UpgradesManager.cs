using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesManager : LocalSingletonBehaviour<UpgradesManager> {
	private Dictionary<string , int> maxUpgradeValues = new Dictionary<string , int>();

	public void DoInit (Dictionary<string , int> maxUpgradeValues) {
		this.maxUpgradeValues = maxUpgradeValues;
	}

	public int GetMaxUpgradeValue (string objectKey) {
		if (maxUpgradeValues.ContainsKey(objectKey)) return maxUpgradeValues[objectKey];
		return 0;
	}
}
