using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerTrapObject : NormalResourceObject {
    //[SerializeField] private ParticleSystem effectOpen;

    public override event Action<Node> OnDestroyed = delegate { };

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

            int score = 0;

            if (IsObstacle) {
                foreach (Resource r in RequiredResources) score += r.Count * 10;
                foreach (Resource r in Resources) score += r.Count * 10;

                AwardHandler.I.UpdateAward("award_obstacles", 1);
            } else {
                foreach (Resource r in Resources) score += r.Count * 10;
            }

            GameManager.I.AddScores(transform.position, score);

            Node.IsFree = true;
            DestroyImmediate(gameObject);
            LevelTaskController.I.UpdateTask(KeyAction, 1);
            AudioWrapper.I.PlaySfx("trap01");

            if (DurationWorkTotal > 1F)
                WorkBar.gameObject.SetActive(false);

            OnDestroyed.Invoke(this.Node);
            LevelTaskPingController.I.DisablePin(this);

            WaitingWorkers = 0;
            SendedCharacters.Clear();
        }
    }
}
