using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAction {
	public List<Resource> NeedResources;
	public Resource GetResources;
	public float Duration;

	public PlayerAction() {
	}

	public PlayerAction(List<Resource> needResources, float duration) {
		NeedResources = needResources;
		Duration = duration;
	}

	public PlayerAction(Resource getResources, float duration) {
		GetResources = getResources;
		Duration = duration;
	}

	public PlayerAction(List<Resource> needResources, Resource getResources, float duration) {
		NeedResources = needResources;
		GetResources = getResources;
		Duration = duration;
	}
}
