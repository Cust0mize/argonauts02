using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapObject : NormalResourceObject {
    [SerializeField] private GameObject trapPart;
    [SerializeField] private ParticleSystem effectOpen;

    public override event Action<Node> OnDestroyed = delegate { };

    private Vector3 medeaPos = Vector3.zero;
    private bool canBeUsed = true;

    public override bool CanBeUsed() {
        return canBeUsed;
    }

    public override IEnumerator Use(Action<List<Resource>> callback) {
        WaitingWorkers++;
        while (WaitingWorkers < CountCharacters()) {
            yield return null;
        }

        if (medeaPos == Vector3.zero)
            medeaPos = GetFirstFreePoint(transform.position, true).transform.position;

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
            canBeUsed = false;

            if (effectOpen != null)
                effectOpen.Play();

            yield return new WaitForSeconds(0.2F);

            int score = 0;

            if (IsObstacle) {
                foreach (Resource r in RequiredResources) score += r.Count * 10;
                foreach (Resource r in Resources) score += r.Count * 10;

                AwardHandler.I.UpdateAward("award_obstacles", 1);
            } else {
                foreach (Resource r in Resources) score += r.Count * 10;
            }

            GameManager.I.AddScores(transform.position, score);

            trapPart.gameObject.SetActive(false);

            GameManager.I.CampObject.SpawnMedea(medeaPos);
            GameResourceManager.I.Medea.BeHappy();

            AudioWrapper.I.PlaySfx("trap02");
            Node.IsFree = true;
            LevelTaskController.I.UpdateTask(KeyAction, 1);

            if (DurationWorkTotal > 1F)
                WorkBar.gameObject.SetActive(false);

            OnDestroyed.Invoke(this.Node);
            LevelTaskPingController.I.DisablePin(this);

            WaitingWorkers = 0;
            SendedCharacters.Clear();
        }
    }
}
