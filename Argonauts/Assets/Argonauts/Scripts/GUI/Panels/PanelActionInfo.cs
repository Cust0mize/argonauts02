using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelActionInfo : MonoBehaviour {
	[SerializeField]
	RectTransform needResources;
	[SerializeField]
	RectTransform incomeResources;

	[SerializeField]
	GameObject prefabElementResources;

	[SerializeField]
	TextMeshProUGUI title;

	bool need = false;
	bool income = false;

    private string lastActionName;
    private List<Resource> lastNeedR;
    private List<Resource> lastIncomeR;

    public void Init (string actionName, List<Resource> needR, List<Resource> incomeR, bool recalculate = true) {
        lastActionName = actionName;
        lastNeedR = needR;
        lastIncomeR = incomeR;

        title.text = actionName;

		List<Resource> need = null;
		List<Resource> income = null;

		if (needR != null)
			need = new List<Resource> (needR);
		if (incomeR != null)
			income = new List<Resource> (incomeR);

		needResources.gameObject.SetActive (true);
		incomeResources.gameObject.SetActive (true);

        bool enought = GameManager.I.RequiredResourcesExistMaxCharacters(needR);
        if (enought)
        {
            needResources.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 80, 255);
        }
        else
        {
            needResources.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(237, 28, 36, 255);
        }

		this.need = (need != null && need.Count > 0);
		this.income = (income != null && income.Count > 0);



		if (this.need) {
            GameObject rPrefab = null;
            int countExistResourcePrefabs = needResources.transform.GetChild(1).childCount;

            foreach(Transform t in needResources.transform.GetChild(1)) {
                t.gameObject.SetActive(false);
            }

            for (int i = 0; i < need.Count; i++) {
                if (i + 1 > countExistResourcePrefabs) {
                    rPrefab = Instantiate(prefabElementResources, needResources.transform.GetChild(1));
                    rPrefab.transform.localScale = Vector3.one;
                    rPrefab.GetComponent<ResourceElementGUI>().Init(need[i], true, GameManager.I.RequiredResourcesExistMaxCharacters(new List<Resource> { need[i] }));
                } else {
                    rPrefab = needResources.transform.GetChild(1).GetChild(i).gameObject;
                    rPrefab.gameObject.SetActive(true);
                    rPrefab.GetComponent<ResourceElementGUI>().Init(need[i], true, GameManager.I.RequiredResourcesExistMaxCharacters(new List<Resource> { need[i] }));
                }
            }
		} else {
			needResources.gameObject.SetActive (false);
		}
		if (this.income) {
            GameObject rPrefab = null;
            int countExistResourcePrefabs = incomeResources.transform.GetChild(1).childCount;

            foreach (Transform t in incomeResources.transform.GetChild(1)) {
                t.gameObject.SetActive(false);
            }

            for (int i = 0; i < income.Count; i++) {
                if (i + 1 > countExistResourcePrefabs) {
                    rPrefab = Instantiate(prefabElementResources, incomeResources.transform.GetChild(1));
                    rPrefab.transform.localScale = Vector3.one;
                    rPrefab.GetComponent<ResourceElementGUI>().Init(income[i]);
                } else {
                    rPrefab = incomeResources.transform.GetChild(1).GetChild(i).gameObject;
                    rPrefab.gameObject.SetActive(true);
                    rPrefab.GetComponent<ResourceElementGUI>().Init(income[i]);
                }
            }
		} else {
			incomeResources.gameObject.SetActive (false);
		}

        if(recalculate) {
            StopAllCoroutines();
            StartCoroutine(CalculateSize());
        }
	}

    public void ReInit() {
        if (string.IsNullOrEmpty(lastActionName)) return;
        Init(lastActionName, lastNeedR, lastIncomeR, false);
    }

	IEnumerator CalculateSize () {
		GetComponent<CanvasGroup> ().alpha = 0F;

		yield return new WaitForEndOfFrame ();

		if (need) {
			needResources.sizeDelta = new Vector2 (needResources.transform.GetChild (1).GetComponent<RectTransform> ().sizeDelta.x, needResources.sizeDelta.y);
		}

		if (income) {
            incomeResources.sizeDelta = new Vector2 (incomeResources.transform.GetChild (1).GetComponent<RectTransform> ().sizeDelta.x, incomeResources.sizeDelta.y);
		}

		yield return new WaitForEndOfFrame ();

		float sizeNeed = 0f;
		float sizeIncome = 0f;

		LayoutRebuilder.ForceRebuildLayoutImmediate (needResources.parent.GetComponent<RectTransform> ());
		LayoutRebuilder.ForceRebuildLayoutImmediate (incomeResources.parent.GetComponent<RectTransform> ());

		if (need)
			sizeNeed = needResources.parent.GetComponent<RectTransform> ().sizeDelta.x;
		if (income)
			sizeIncome = incomeResources.parent.GetComponent<RectTransform> ().sizeDelta.x;

		yield return new WaitForEndOfFrame ();

		needResources.parent.parent.GetComponent<RectTransform> ().sizeDelta = new Vector2 (20 + Mathf.Max (sizeNeed, sizeIncome), Mathf.Abs (needResources.parent.GetComponent<RectTransform> ().anchoredPosition.y) + needResources.parent.GetComponent<RectTransform> ().sizeDelta.y + 13);

		GetComponent<CanvasGroup> ().alpha = 1F;

        yield return null;
	}
}
