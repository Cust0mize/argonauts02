using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BridgeObject : NormalResourceObject {
	[SerializeField] private Sprite bridgeActive;
	[SerializeField] private Sprite bridgeInactive;
	[SerializeField] private bool isActive;
    [SerializeField] private ParticleSystem effectOpen;

	private SpriteRenderer spriteRenderer;

	public bool IsActive {
		get {
			return isActive;
		}set {
			isActive = value; 
			SpriteRenderer.sprite = value ? bridgeActive : bridgeInactive;
		}
	}

	public SpriteRenderer SpriteRenderer {
		get {
			if (spriteRenderer == null) {
				spriteRenderer = GetComponent<SpriteRenderer> ();
			}
			return spriteRenderer;
		}
	}

	public override bool CanBeUsed () {
		return !IsActive;
	}

    public override event Action<Node> OnDestroyed = delegate { };

	public override void Start () {
		base.Start ();

		IsActive = IsActive;
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

            Open();

            int score = 0;

            foreach (Resource r in RequiredResources) score += r.Count * 10;
            foreach (Resource r in Resources) score += r.Count * 10;

            AwardHandler.I.UpdateAward("award_obstacles", 1);

            GameManager.I.AddScores(transform.position, score);

            LevelTaskController.I.UpdateTask(KeyAction, 1);

            LevelTaskPingController.I.DisablePin(this);

            if (DurationWorkTotal > 1F)
                WorkBar.gameObject.SetActive(false);

            PlayOnEndUse();
        }

		yield return null;
	}

	public void Open () {
        if (effectOpen != null)
            effectOpen.Play();

		IsActive = true;
		Node.IsFree = true;

        OnDestroyed.Invoke(this.Node);

        StopPing();
	}
}
