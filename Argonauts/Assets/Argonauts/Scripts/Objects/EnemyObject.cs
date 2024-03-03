using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyObject : BaseInObject {
    public enum EnemyTypes { Ciclop, Minotaur, Ghost }
    [SerializeField] private EnemyTypes enemyType;

	[SerializeField]
	protected string keyAction;

    public float DurationWork = 5f;

    public float DurationWorkTotal {
        get {
            return DurationWork;
        }
    }

    public override event Action<Node> OnDestroyed = delegate { };

    protected override void Awake() {
		base.Awake();

		PlayerAction pl = ConfigManager.Instance.GetPlayerAction(keyAction);
		RequiredResources = new List<Resource>(pl.NeedResources);
        UpdateRequiredResourcesWithBonus(RequiredResources);

        CountStages = CountCharacters();

        DurationWork = ConfigManager.Instance.GetDurationAction(keyAction);

        GetComponent<Animator>().Play("Idle", 0, Random.Range(0, 10f));
	}

    private void UpdateRequiredResourcesWithBonus(List<Resource> resources)
    {
        if (!GameBonusesManager.I.BonuseIsActive(GameBonus.GameBonusTypes.EnemyCost)) return;

        int countResources = 0;
        foreach (Resource resource in resources) {
            countResources += resource.Count;
        }

        int resourceSale = Mathf.RoundToInt(countResources * 0.25F);

        foreach (Resource resource in resources) {
            while (resource.Count > 1 && resourceSale > 0) {
                resource.Count--;
                resourceSale--;
            }
        }
    }

    private void Update()
    {
        if (CanPing && Overlay != null) {
            Overlay.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        }
    }

    public override void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData) {
		if (!Available)
			return;

        StringBuilder sb = new StringBuilder();
        string[] keyParts = keyAction.Split('_');
        for (int i = 1; i < keyParts.Length; i++) {
            sb.Append("_");
            sb.Append(keyParts[i]);
        }

		GameGUIManager.I.ShowActionInfo(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb)), RequiredResources, null, this);
	}

	public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData) {
		base.OnPointerClick(eventData);

		if (!Available)
			return;

#if !UNITY_STANDALONE
		GameGUIManager.I.OnObjectClickedMobile (LocalizationManager.GetLocalizedString(string.Format("title_kill_{0}" , keyAction.Split('_')[2])) , RequiredResources , null , this);
#endif
	}

	public override IEnumerator Use(System.Action<List<Resource>> callback) {
        WaitingWorkers++;
        while (WaitingWorkers < CountCharacters()) {
            yield return null;
        }

        if (GetComponent<Animator>().HasBool("Attack"))
            GetComponent<Animator>().SetBool("Attack", true);

        float timer = DurationWork;
        while (timer >= 0) {
            timer -= DeltaTimeWork;
            yield return null;
        }

        CountStages--;
        if (CountStages <= 0) {
            Node.IsFree = true;

            LevelTaskController.I.UpdateTask(keyAction, 1);
            AwardHandler.I.UpdateAward("award_victory", 1);

            int score = 0;
            foreach (Resource r in RequiredResources) score += r.Count * 10;
            GameManager.I.AddScores(transform.position, score);

            PlayOnEndUse();

            if (enemyType == EnemyTypes.Ciclop || enemyType == EnemyTypes.Minotaur) {
                AudioWrapper.I.PlaySfx("enemy_die");
            } else {
                AudioWrapper.I.PlaySfx("ghost_die");
            }

            OnDestroyed.Invoke(Node);
            DestroyImmediate(gameObject);

            WaitingWorkers = 0;
            SendedCharacters.Clear();
            CountStages = CountCharacters();
        }
	}
}
