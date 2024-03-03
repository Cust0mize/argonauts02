using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ProductionBuildObject : BuildObject {
	[SerializeField]
	private float productionDuration = 3F;

	public List<Resource> DropResources;
	public ResourcesDropPoint DropPoint;

    [SerializeField]
    private GameObject incomeBarPrefab;
    private HudBar incomeWorkdBar;

    private HudBar IncomeWorkBar {
        get {
            if (incomeWorkdBar == null) {
                GameObject b = Instantiate(incomeBarPrefab, GameManager.I.Canvas2D.transform);
                incomeWorkdBar = b.GetComponent<HudBar>();
                incomeWorkdBar.transform.position = transform.position;
                incomeWorkdBar.transform.position = new Vector2(incomeWorkdBar.transform.position.x + offsetWorkBar.x, incomeWorkdBar.transform.position.y + offsetWorkBar.y);
                b.SetActive(false);
            }
            return incomeWorkdBar;
        }
    }

    protected override IEnumerator DelayStart()
    {
        while (!GameManager.I.GameInited)
            yield return null;

        if (CanUpgrade) {
            RequiredResources = ConfigManager.Instance.GetPlayerAction(KeyAction).NeedResources;
            UpdateRequiredResourcesWithBonus(RequiredResources);

            CountStages = CountCharacters();
            if (iconAvailableUpgradeBar != null)
                iconAvailableUpgradeBar.IsAvailable = GameManager.I.RequiredResourcesExistMaxCharacters(RequiredResources);
        }

        UpdateUpgradeIconState();
    }

    private void UpdateRequiredResourcesWithBonus(List<Resource> resources)
    {
        if (!GameBonusesManager.I.BonuseIsActive(GameBonus.GameBonusTypes.BuildsCost)) return;

        int countResources = 0;
        foreach(Resource resource in resources) {
            countResources += resource.Count;
        }

        int resourceSale = Mathf.RoundToInt(countResources * 0.25F);

        foreach(Resource resource in resources) {
            while(resource.Count > 1 && resourceSale > 0) {
                resource.Count--;
                resourceSale--;
            }
        }
    }

    public void StartWork () {
		if (Upgrade > 0)
			StartCoroutine (Worker ());
	}

    public override void OnPointerEnter (UnityEngine.EventSystems.PointerEventData eventData) {
        if (!Available)
            return;
        if (!CanUpgrade)
            return;

        string key = KeyAction.Split('_')[2];
        key = key.Substring(0, key.Length - 2);

        string fullkey = string.Format("{0}{1:00}", key, Upgrade + 1);

        ProductionBuildObject obj = ResourceManager.I.GetObjectPrefab(fullkey).GetComponent<ProductionBuildObject>();
        if (obj == null) {
            return;
        }

        StringBuilder sb = new StringBuilder();
        string[] keyParts = KeyAction.Split('_');
        for (int i = 1; i < keyParts.Length; i++) {
            sb.Append("_");
            sb.Append(keyParts[i]);
        }

        GameGUIManager.I.ShowActionInfo(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb)), RequiredResources, obj.DropResources, this);
    }

    public override void OnPointerClick (UnityEngine.EventSystems.PointerEventData eventData) {
        base.OnPointerClick(eventData);

        if (!Available)
            return;
        if (!CanUpgrade)
            return;

        string key = KeyAction.Split('_')[2];
        string fullkey = string.Format("{0}{1:00}", key.Substring(0, key.Length - 2), Upgrade + 1);

        ProductionBuildObject obj = ResourceManager.I.GetObjectPrefab(fullkey).GetComponent<ProductionBuildObject>();
        if(obj == null) {
            return;
        }

#if !UNITY_STANDALONE
        StringBuilder sb = new StringBuilder();
        string[] keyParts = KeyAction.Split('_');
        for (int i = 1; i < keyParts.Length; i++) {
            sb.Append("_");
            sb.Append(keyParts[i]);
        }

        GameGUIManager.I.OnObjectClickedMobile(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb)), RequiredResources, obj.DropResources, this);
#endif
    }

    private IEnumerator Worker () {
		float productionTimer = 0f;

        while (!GameManager.I.GameStarted) yield return null;

		while (true) {
			while (Upgrade < 1) {
				yield return null;
			}
			while (!DropPoint.IsEmpty) {
				yield return null;
			}
            IncomeWorkBar.gameObject.SetActive (true);
			while (productionTimer < productionDuration) {
                IncomeWorkBar.UpdateBar (productionTimer, productionDuration, false);
				productionTimer += Time.deltaTime;
				yield return null;
			}
            IncomeWorkBar.gameObject.SetActive (false);
            DropPoint.Drop (DropResources, transform.position);
			productionTimer = 0f;
			yield return null;
		}
	}

    protected override void OnDestroy() {
        if (incomeWorkdBar != null && incomeWorkdBar.gameObject != null)
            Destroy(IncomeWorkBar.gameObject);

        base.OnDestroy();
    }
}
