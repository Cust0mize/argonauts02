using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyObject : NormalResourceObject {
    [SerializeField] private GameObject keyPrefab;

    [HideInInspector] public string GateID;
    private GateObject Gate;

    public override void Start() {
        base.Start();

        StartCoroutine(DelayInit());
    }

	public override IEnumerator Use (System.Action<List<Resource>> callback) {
        WaitingWorkers++;
        while (WaitingWorkers < CountCharacters()) {
            yield return null;
        }

        CountStages--;
        if (CountStages <= 0) {

            PlayOnEndUse();

            int score = 0;
            foreach (Resource r in RequiredResources) score += r.Count * 10;
            foreach (Resource r in Resources) score += r.Count * 10;
            GameManager.I.AddScores(transform.position, score);

            Vector3 pos = transform.position;
            yield return base.Use(callback);
            Gate.DoOpen(pos, keyPrefab);

            WaitingWorkers = 0;
            SendedCharacters.Clear();
            CountStages = CountCharacters();
        }
	}

    private IEnumerator DelayInit() {
        while (!GameManager.I.GameInited)
            yield return null;
        Gate = GameManager.I.GetObject(GateID).GetComponent<GateObject>();
    }
}
