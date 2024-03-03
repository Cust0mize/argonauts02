using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NormalResourceObject : BaseResourceObject {
    [SerializeField] protected bool IsObstacle = false;
    [SerializeField] private ParticleSystem effectTrace;

    public override event Action<Node> OnDestroyed = delegate { };

	protected override void Awake () {
		base.Awake ();

        if(!string.IsNullOrEmpty(KeyAction)) {
            PlayerAction pl = ConfigManager.Instance.GetPlayerAction(KeyAction);
            if (pl.NeedResources != null) {
                RequiredResources = new List<Resource>(pl.NeedResources);
                CountStages = CountCharacters();
            }
            if (pl.GetResources != null)
                Resources = new List<Resource>() { pl.GetResources };

            DurationWork = ConfigManager.Instance.GetDurationAction(KeyAction);
        }
	}

	public override void OnPointerEnter (UnityEngine.EventSystems.PointerEventData eventData) {
		if (!Available || !Clickable)
			return;

        StringBuilder sb = new StringBuilder();
        string[] keyParts = KeyAction.Split('_');
        for (int i = 1; i < keyParts.Length; i++) {
            sb.Append("_");
            sb.Append(keyParts[i]);
        }

        switch (KeyAction.Substring(0, KeyAction.Length - 2)) {
            case "act_take_food":GameGUIManager.I.ShowActionInfo(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb.ToString().Substring(0, sb.Length - 2))), RequiredResources, Resources, this);break;
            case "act_take_wood":GameGUIManager.I.ShowActionInfo(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb.ToString().Substring(0, sb.Length - 2))), RequiredResources, Resources, this);break;
            case "act_take_stone":GameGUIManager.I.ShowActionInfo(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb.ToString().Substring(0, sb.Length - 2))), RequiredResources, Resources, this);break;
            case "act_take_gold":GameGUIManager.I.ShowActionInfo(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb.ToString().Substring(0, sb.Length - 2))), RequiredResources, Resources, this);break;
            case "act_take_tooth":GameGUIManager.I.ShowActionInfo(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb.ToString().Substring(0, sb.Length - 2))), RequiredResources, Resources, this); break;
            default:GameGUIManager.I.ShowActionInfo(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb)), RequiredResources, Resources, this);break;
        }
	}

	public override void OnPointerClick (UnityEngine.EventSystems.PointerEventData eventData) {
		base.OnPointerClick (eventData);

		if (!Available || !Clickable)
			return;

#if !UNITY_STANDALONE
        StringBuilder sb = new StringBuilder();
        string[] keyParts = KeyAction.Split('_');
        for (int i = 1; i < keyParts.Length; i++) {
            sb.Append("_");
            sb.Append(keyParts[i]);
        }

        switch (KeyAction.Substring(0, KeyAction.Length - 2)) {
            case "act_take_food": GameGUIManager.I.OnObjectClickedMobile(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb.ToString().Substring(0, sb.Length - 2))), RequiredResources, Resources, this); break;
            case "act_take_wood": GameGUIManager.I.OnObjectClickedMobile(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb.ToString().Substring(0, sb.Length - 2))), RequiredResources, Resources, this); break;
            case "act_take_stone": GameGUIManager.I.OnObjectClickedMobile(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb.ToString().Substring(0, sb.Length - 2))), RequiredResources, Resources, this); break;
            case "act_take_gold": GameGUIManager.I.OnObjectClickedMobile(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb.ToString().Substring(0, sb.Length - 2))), RequiredResources, Resources, this); break;
            case "act_take_tooth": GameGUIManager.I.OnObjectClickedMobile(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb.ToString().Substring(0, sb.Length - 2))), RequiredResources, Resources, this); break;
            default: GameGUIManager.I.OnObjectClickedMobile(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb)), RequiredResources, Resources, this); break;
        }
#endif
    }

    public override IEnumerator Use (Action<List<Resource>> callback) {
        WaitingWorkers++;
        while (WaitingWorkers < CountCharacters()) {
            yield return null;
        }

		if (DurationWorkTotal > 1F)
			WorkBar.gameObject.SetActive (true);

        bool effectEndPlayed = false;

		float timer = DurationWorkTotal;
		while (timer >= 0) {
            timer -= DeltaTimeWork;
			if (DurationWorkTotal > 1F)
				WorkBar.UpdateBar (DurationWorkTotal - timer, DurationWorkTotal, false);

            if(!effectEndPlayed && timer <= 0.1f){
                PlayOnEndUse();
                effectEndPlayed = true;
            } 
			yield return null;
		}

		CountStages--;
		if (CountStages <= 0) {
            int score = 0;

            if (IsObstacle) {
                foreach (Resource r in RequiredResources) score += r.Count * 10;
                foreach (Resource r in Resources) score += r.Count * 10;

                AwardHandler.I.UpdateAward("award_obstacles", 1);
            } else {
                foreach (Resource r in Resources) score += r.Count * 10;
            }

            GameManager.I.AddScores(transform.position, score);

            callback.Invoke(CheckGemInResources(ObjectCopier.Clone(Resources)));

			Node.IsFree = true;
			DestroyImmediate (gameObject);
            switch (KeyAction.Substring(0, KeyAction.Length - 2)) {
                case "act_take_food": break;
                case "act_take_wood": break;
                case "act_take_stone": break;
                case "act_take_gold": break;
                default:
                    LevelTaskController.I.UpdateTask(KeyAction, 1);
                    break;
            }

            if (DurationWorkTotal > 1F)
                WorkBar.gameObject.SetActive(false);

            OnDestroyed.Invoke(this.Node);
            LevelTaskPingController.I.DisablePin(this);

            WaitingWorkers = 0;
            SendedCharacters.Clear();
            CountStages = CountCharacters();
		}
	}

    public void SetActiveTrace(bool value) {
        if (value) {
            effectTrace.Clear();
            effectTrace.Play(true);
        }
        else effectTrace.Stop();
    }

    public void SetEmissionEnable(bool value) {
        //effectTrace..enabled = value;
    }

    private List<Resource> CheckGemInResources(List<Resource> resources) {
        List<Resource> gemsToRemove = new List<Resource>();

        foreach(Resource r in resources) {
            if(r.Type == Resource.Types.Gem) {
                gemsToRemove.Add(r);
            }
        }
        
        foreach(Resource r in gemsToRemove) {
            resources.Remove(r);
        }

        if (UserData.I.GetLevelDump(GameManager.I.LevelNumber) == null) GameManager.I.AddResources(gemsToRemove, true);

        return resources;
    }
}
