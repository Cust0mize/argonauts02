using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ObstacleObject : NormalResourceObject {
    public override event Action<Node> OnDestroyed = delegate { };

	public override void OnPointerEnter (UnityEngine.EventSystems.PointerEventData eventData) {
		if (!Available)
			return;

        StringBuilder sb = new StringBuilder();
        string[] keyParts = KeyAction.Split('_');
        for (int i = 1; i < keyParts.Length; i++) {
            sb.Append("_");
            sb.Append(keyParts[i]);
        }

        GameGUIManager.I.ShowActionInfo (LocalizationManager.GetLocalizedString(string.Format("title{0}", sb)), RequiredResources, Resources, this);
	}

	public override IEnumerator Use (System.Action<List<Resource>> callback) {
        WaitingWorkers++;
        while (WaitingWorkers < CountCharacters()) {
            yield return null;
        }

        if (DurationWorkTotal > 1F)
            WorkBar.gameObject.SetActive(true);
        
        float timer = DurationWorkTotal;
        while (timer >= 0) {
            timer -= DeltaTimeWork;
            if (DurationWorkTotal > 1F)
                WorkBar.UpdateBar(DurationWorkTotal - timer, DurationWorkTotal, false);
            yield return null;
        }

        CountStages--;
        if (CountStages <= 0) {
            callback.Invoke(ObjectCopier.Clone(Resources));
            Node.IsFree = true;

            PlayOnEndUse();

            int score = 0;

            foreach (Resource r in RequiredResources) score += r.Count * 10;
            foreach (Resource r in Resources) score += r.Count * 10;

            AwardHandler.I.UpdateAward("award_obstacles", 1);

            GameManager.I.AddScores(transform.position, score);

            DestroyImmediate(gameObject);

            LevelTaskController.I.UpdateTask(KeyAction, 1);
            AwardHandler.I.UpdateAward("award_obstacles", 1);

            if (DurationWorkTotal > 1F)
                WorkBar.gameObject.SetActive(false);

            OnDestroyed.Invoke(this.Node);
            LevelTaskPingController.I.DisablePin(this);

            WaitingWorkers = 0;
            SendedCharacters.Clear();
            CountStages = CountCharacters();
        }
	}
}
