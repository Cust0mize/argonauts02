using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MarketPlaceObject : BaseInObject {
    public List<Resource> Resources;
    private float DurationWork;

    protected override void Awake() {
        base.Awake();

        if (!string.IsNullOrEmpty(KeyAction)) {
            PlayerAction pl = ConfigManager.Instance.GetPlayerAction(KeyAction);
            if (pl.NeedResources != null) {
                RequiredResources = new List<Resource>(pl.NeedResources);
            }
            if (pl.GetResources != null) {
                Resources = new List<Resource>() { pl.GetResources };
                UpdateResourcesWithBonus(Resources);
            }

            DurationWork = ConfigManager.Instance.GetDurationAction(KeyAction);
        }
    }

    private void UpdateResourcesWithBonus(List<Resource> resources)
    {
        if (!GameBonusesManager.I.BonuseIsActive(GameBonus.GameBonusTypes.Market)) return;

        foreach (Resource resource in resources) {
            resource.Count++;
        }
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

        GameGUIManager.I.ShowActionInfo(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb)), RequiredResources, Resources, this);
	}

    public override void CheckShouldPing()
    { }

    protected override IEnumerator NoResourcePing(int pulsationCount, float pulsationSpeed)
    {
        if (Overlay == null) yield break;

        Overlay.gameObject.SetActive(true);

        Color col = Overlay.color;

        noResourcePing = true;

        bool increase = true;
        int pulsationCountCurrent = 0;
        float value = 0F;

        while (pulsationCountCurrent < pulsationCount)
        {
            if (increase)
            {
                if (value > 1F)
                    increase = false;
                else
                    value += Time.fixedDeltaTime * pulsationSpeed;
            }
            else
            {
                if (value < 0F)
                {
                    increase = true;
                    pulsationCountCurrent++;
                }
                else
                    value -= Time.fixedDeltaTime * pulsationSpeed;
            }

            col.a = value;
            Overlay.color = col;

            yield return null;
        }

        noResourcePing = false;

        StopPing();
    }

	public override void OnPointerClick (UnityEngine.EventSystems.PointerEventData eventData) {
		base.OnPointerClick (eventData);

		if (!Available)
			return;

        StringBuilder sb = new StringBuilder();
        string[] keyParts = KeyAction.Split('_');
        for (int i = 1; i < keyParts.Length; i++) {
            sb.Append("_");
            sb.Append(keyParts[i]);
        }

		#if !UNITY_STANDALONE
        GameGUIManager.I.OnObjectClickedMobile (LocalizationManager.GetLocalizedString(string.Format("title{0}", sb)), RequiredResources, Resources, this);
		#endif
	}

	public override IEnumerator Use (System.Action<List<Resource>> callback) {
        float timer = DurationWork;
        while (timer >= 0) {
            timer -= DeltaTimeWork;
            yield return null;
        }

        callback.Invoke(ObjectCopier.Clone(Resources));
        AwardHandler.I.UpdateAward("award_trader", 1);
        PlayOnEndUse();

        int score = 0;
        foreach (Resource r in Resources) score += r.Count * 20;
        GameManager.I.AddScores(transform.position, score);

        WaitingWorkers = 0;
        SendedCharacters.Clear();
        OrderCounts--;

        StartCoroutine(IEDelayShowPopupResources());
    }

    private IEnumerator IEDelayShowPopupResources() {
        yield return new WaitForSeconds(0.4F);
        GameGUIManager.I.ShowPopupResources(transform.position, GameResourceManager.I.ChangeSign(new List<Resource>(RequiredResources)));
    }
}
