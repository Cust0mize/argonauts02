using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelNoResources : MonoBehaviour {
	[SerializeField]
	RectTransform needResources;

	[SerializeField]
	GameObject prefabElementResources;

	public void Init (List<Resource> noResourecs) {
		List<Resource> need = null;

        if (noResourecs != null)
            need = new List<Resource> (noResourecs);

		needResources.gameObject.SetActive (true);

        GameObject rPrefab = null;
        int countExistResourcePrefabs = needResources.childCount;

        foreach (Transform t in needResources.transform) {
            t.gameObject.SetActive(false);
        }

        List<Resource> needDelta = GameResourceManager.I.GetDeltaResource(need, GameResourceManager.I.Resources);

        for (int i = 0; i < need.Count; i++) {
            if (GameResourceManager.I.RequiredResourcesExistMaxCharacters(new List<Resource>() { need[i] })) continue;

            if (i + 1 > countExistResourcePrefabs) {
                rPrefab = Instantiate(prefabElementResources, needResources.transform);
                rPrefab.transform.localScale = Vector3.one;
                rPrefab.GetComponent<ResourceElementGUI>().Init(needDelta[i], true, false);
            } else {
                rPrefab = needResources.GetChild(i).gameObject;
                rPrefab.gameObject.SetActive(true);
                rPrefab.GetComponent<ResourceElementGUI>().Init(needDelta[i], true, false);
            }
        }

		StartCoroutine (CalculateSize ());
	}

	IEnumerator CalculateSize () {
        GetComponent<CanvasGroup>().alpha = 0f;
        yield return new WaitForEndOfFrame();
        GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Clamp(50 + needResources.GetComponent<RectTransform>().sizeDelta.x, 200, float.MaxValue),  GetComponent<RectTransform>().sizeDelta.y);
        GetComponent<CanvasGroup>().alpha = 1f;
    }
}
