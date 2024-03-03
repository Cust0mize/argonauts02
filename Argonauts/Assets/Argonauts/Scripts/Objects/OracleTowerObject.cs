using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OracleTowerObject : BuildObject {
    public override event Action<int> OnSuccessUpgrade = delegate { };
    [SerializeField] private string OracleTowerObjectKey;

    public override event Action<Node> OnDestroyed = delegate { };

    public override string BuildKey {
        get {
            return OracleTowerObjectKey;
        }
    }

    protected override void Awake() {
        base.Awake();
    }

    protected override void OnUpgrade(int upgrade) {
        LevelTaskController.I.UpdateTask(KeyAction, 1);
        AwardHandler.I.UpdateAward("award_builder", 1);
        GameManager.I.AddScores(transform.position, upgrade * 50);
        OnSuccessUpgrade.Invoke(upgrade);
    }
}
