using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SwampObject : NormalResourceObject {
	[SerializeField] Sprite pepperedSwamp;
	[SerializeField]
	private bool isPeppered;

    public override event Action<Node> OnDestroyed = delegate { };

	public override bool CanBeUsed () {
		return !isPeppered;
	}

    public override void Start () {
		if (isPeppered) {
			GetComponent<SpriteRenderer> ().sprite = pepperedSwamp;
			if (ping != null) {
				StopCoroutine (ping);
				ping = null;
			}
			Overlay.gameObject.SetActive (false);
		}
	}

	public override IEnumerator Use (System.Action<List<Resource>> callback) {
        WaitingWorkers++;
        while (WaitingWorkers < CountCharacters()) {
            yield return null;
        }

		if (DurationWorkTotal > 1F)
			WorkBar.gameObject.SetActive (true);

		float timer = DurationWorkTotal;
		while (timer >= 0) {
            timer -= DeltaTimeWork;
			if (DurationWorkTotal > 1F)
				WorkBar.UpdateBar (DurationWorkTotal - timer, DurationWorkTotal, false);
			yield return null;
		}
			
        CountStages--;
        if (CountStages <= 0) {
            callback.Invoke(ObjectCopier.Clone(Resources));
            GetComponent<SpriteRenderer>().sprite = pepperedSwamp;
            isPeppered = true;
            Node.IsFree = true;

            if (ping != null) {
                StopCoroutine(ping);
                ping = null;
            }

            int score = 0;

            foreach (Resource r in RequiredResources) score += r.Count * 10;
            foreach (Resource r in Resources) score += r.Count * 10;
            GameManager.I.AddScores(transform.position, score);

            Overlay.gameObject.SetActive(false);

            LevelTaskController.I.UpdateTask(KeyAction, 1);
            AwardHandler.I.UpdateAward("award_obstacles", 1);

            if (DurationWorkTotal > 1F)
                WorkBar.gameObject.SetActive(false);

            OnDestroyed.Invoke(this.Node);
            LevelTaskPingController.I.DisablePin(this);

            PlayOnEndUse();

            WaitingWorkers = 0;
            SendedCharacters.Clear();
            CountStages = CountCharacters();
        }
	}
}
