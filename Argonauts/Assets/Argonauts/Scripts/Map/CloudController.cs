using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map {
	public class CloudController : MonoBehaviour {
		[SerializeField] private int[] cloudPrefabsID;
		[SerializeField] private Transform parent;
		[SerializeField] private Vector3 minOffsetSpawn;
		[SerializeField] private Vector3 maxOffsetSpawn;
		[SerializeField] private int maxCloudsSameTime;

		private List<GameObject> usedClouds = new List<GameObject>();

		private void Awake() {
			StartCoroutine(Worker());
		}

		private IEnumerator Worker() {
			while (true) {
				if (usedClouds.Count < maxCloudsSameTime) {
					usedClouds.Add(ObjectPool.I.GetItem(cloudPrefabsID[Random.Range(0, cloudPrefabsID.Length - 1)], parent));
					usedClouds[usedClouds.Count - 1].GetComponent<CloudObject>().OnFadeOut += OnFadeOut;
					usedClouds[usedClouds.Count - 1].transform.localPosition = new Vector3(Random.Range(minOffsetSpawn.x, maxOffsetSpawn.x), Random.Range(minOffsetSpawn.y, maxOffsetSpawn.y), Random.Range(minOffsetSpawn.z, maxOffsetSpawn.z));
					usedClouds[usedClouds.Count - 1].GetComponent<CloudObject>().StartMove();
				}
				yield return null;
			}
		}

		private void OnFadeOut(CloudObject cloud) {
			cloud.OnFadeOut -= OnFadeOut;
			ObjectPool.I.FreeItem(cloud.gameObject);
			usedClouds.Remove(cloud.gameObject);
		}
	}
}
