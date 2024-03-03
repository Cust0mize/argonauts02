using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BuildObject : BaseInObject {
	[SerializeField]
	private float buildingDuration = 3F;
	[SerializeField]
	protected IconAvailableUpgrade iconAvailableUpgradeBar;

    public BuildObjectController BuildObjectController;

    private bool canUpgrade = false;
    public virtual bool CanUpgrade {
        get {
            return canUpgrade;
        }
        set {
            canUpgrade = value;

            if (iconAvailableUpgradeBar != null)
            {
                iconAvailableUpgradeBar.LevelUpgrade = Upgrade;
                if (value) {
                    iconAvailableUpgradeBar.IsAvailable = GameManager.I.RequiredResourcesExistMaxCharacters(RequiredResources);
                }
            }
        }
    }
	public int Upgrade = 1;

	public float BuildingDuration {
		get {
			return buildingDuration;
		}
	}

    public virtual string BuildKey {
        get {
            return KeyAction;
        }
    }

	public virtual event Action<int> OnSuccessUpgrade = delegate { };

	public override void Start () {
        buildingDuration = ConfigManager.Instance.GetDurationAction (KeyAction);
		StartCoroutine (DelayStart ());
	}

	protected virtual IEnumerator DelayStart () {
		while (!GameManager.I.GameInited)
			yield return null;

		if (CanUpgrade) {
            RequiredResources = ConfigManager.Instance.GetPlayerAction (KeyAction).NeedResources;
            CountStages = CountCharacters();
			if (iconAvailableUpgradeBar != null)
				iconAvailableUpgradeBar.IsAvailable = GameManager.I.RequiredResourcesExistMaxCharacters (RequiredResources);
		}

        UpdateUpgradeIconState();
	}

    protected override void OnResourcesChanged (List<Resource> resources) {
        base.OnResourcesChanged(resources);

        if (iconAvailableUpgradeBar != null)
            iconAvailableUpgradeBar.IsAvailable = GameManager.I.RequiredResourcesExistMaxCharacters(RequiredResources);
    }

	public override void OnPointerEnter (UnityEngine.EventSystems.PointerEventData eventData) {
		if (!Available)
			return;

        StringBuilder sb = new StringBuilder();
        string[] keyParts = KeyAction.Split('_');
        for (int i = 1; i < keyParts.Length; i++) {
            sb.Append("_");
            sb.Append(keyParts[i]);
        }

		if (CanUpgrade)
			GameGUIManager.I.ShowActionInfo (LocalizationManager.GetLocalizedString (string.Format ("title{0}", sb)), RequiredResources, null, this);
	}

	public override void OnPointerClick (UnityEngine.EventSystems.PointerEventData eventData) {
		base.OnPointerClick (eventData);

		if (!Available)
			return;

        UpdateUpgradeIconState();

#if !UNITY_STANDALONE
        StringBuilder sb = new StringBuilder();
        string[] keyParts = KeyAction.Split('_');
        for (int i = 1; i < keyParts.Length; i++) {
            sb.Append("_");
            sb.Append(keyParts[i]);
        }

        GameGUIManager.I.OnObjectClickedMobile(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb)), RequiredResources, null, this);
#endif
    }

    protected void UpdateUpgradeIconState() {
        if (iconAvailableUpgradeBar)
            iconAvailableUpgradeBar.gameObject.SetActive(CanUpgrade && !OverseerController.I.ObjectInOrder(this));
    }

	public override IEnumerator Use (System.Action<List<Resource>> callback) {
        WaitingWorkers++;
        while (WaitingWorkers < CountCharacters()) {
            yield return null;
        }

		float buildingTimer = 0F;

		if (BuildingDuration > 1F)
			WorkBar.gameObject.SetActive (true);
		while (buildingTimer < BuildingDuration) {
			if (BuildingDuration > 1F)
				WorkBar.UpdateBar (buildingTimer, BuildingDuration, false);
            buildingTimer += DeltaTimeWork;
			yield return null;
		}
		if (BuildingDuration > 1F)
			WorkBar.gameObject.SetActive (false);

        CountStages--;
        if (CountStages <= 0) {
            PlayOnEndUse();
            OnUpgrade(Upgrade + 1);

            WaitingWorkers = 0;
            SendedCharacters.Clear();
            CountStages = CountCharacters();
        }

		yield return null;
	}

	protected virtual void OnUpgrade (int upgrade) {
        LevelTaskController.I.UpdateTask (string.Format ("{0}{1:00}", KeyAction.Substring(0, KeyAction.Length - 2), upgrade), 1);
        AwardHandler.I.UpdateAward("award_builder", 1);

        GameManager.I.AddScores(transform.position, upgrade * 50);

		OnSuccessUpgrade.Invoke (upgrade);
        UpdateUpgradeIconState();
	}

	public override bool CanBeUsed () {
		return CanUpgrade;
	}

    protected override void OnDestroy () {
        base.OnDestroy();
	}
}
