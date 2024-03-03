using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseObject : NormalResourceObject {
    [SerializeField] private Sprite usedHouseSprite;

    private bool destroyed = false;
    public override event Action<Node> OnDestroyed = delegate { };

    public override bool CanBeUsed() {
        return !destroyed;
    }

    protected override void Awake() {
        base.Awake();
    }

    public override void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData) {
        if (!destroyed)
            base.OnPointerEnter(eventData);
    }

    public override IEnumerator Use(Action<List<Resource>> callback) {
        WaitingWorkers++;
        while (WaitingWorkers < CountCharacters()) {
            yield return null;
        }

        if (DurationWorkTotal > 1F)
            WorkBar.gameObject.SetActive(true);

        bool effectEndPlayed = false;

        float timer = DurationWorkTotal;
        while (timer >= 0) {
            timer -= DeltaTimeWork;
            if (DurationWorkTotal > 1F)
                WorkBar.UpdateBar(DurationWorkTotal - timer, DurationWorkTotal, false);

            if (!effectEndPlayed && timer <= 0.1f) {
                PlayOnEndUse();
                effectEndPlayed = true;
            }
            yield return null;
        }

        CountStages--;
        if (CountStages <= 0) {
            GameManager.I.CampObject.SpawnFakeWorker(transform.position, Node);

            callback.Invoke(ObjectCopier.Clone(Resources));
            Node.IsFree = true;

            GameManager.I.AddScores(transform.position, 200);

            LevelTaskController.I.UpdateTask(KeyAction, 1);

            if (DurationWorkTotal > 1F)
                WorkBar.gameObject.SetActive(false);

            OnDestroyed.Invoke(this.Node);
            destroyed = true;
            LevelTaskPingController.I.DisablePin(this);
            GetComponent<SpriteRenderer>().sprite = usedHouseSprite;

            WaitingWorkers = 0;
            SendedCharacters.Clear();
        }
    }
}
