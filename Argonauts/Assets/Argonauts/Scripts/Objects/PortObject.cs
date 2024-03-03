using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortObject : BuildObject {
    public override event Action<int> OnSuccessUpgrade = delegate { };

    [SerializeField] private string PortKey;

    public override event Action<Node> OnDestroyed = delegate { };

    public override string BuildKey {
        get {
            return PortKey;
        }
    }

    protected override void Awake() {
        base.Awake();

        StartCoroutine(IECheckFreeNode());
    }

    private IEnumerator IECheckFreeNode() {
        yield return new WaitForSeconds(0.1f);
        if (Upgrade == 1) {
            Node.IsFree = true;
            OnDestroyed.Invoke(Node);
        }
    }

    public void EquipBoat(BaseCharacter character, Vector2 nextNodePos, Node nextNode)
    {
        character.transform.position = GetNearOutPoint(nextNodePos).Position;
        character.BoatEquiped = GameManager.I.OnRiver(nextNode);
    }

    public void UnequipBoat(BaseCharacter character, Vector2 nextNodePos, Node nextNode)
    {
        character.transform.position = GetFirstFreePoint(nextNodePos, false).transform.position;
        character.BoatEquiped = GameManager.I.OnRiver(nextNode);
    }

    public Node GetNearOutPoint(Vector2 pos)
    {
        List<Node> points = (BuildObjectController.CustomData as List<Node>);

        float distance = float.MaxValue;
        int index = -1;

        int countPoints = points.Count;
        for (int i = 0; i < countPoints; i++) {
            float d = Vector3.Distance(WorkPoints[i].transform.position, pos);

            if (d < distance) {
                index = i;
                distance = d;
            }
        }

        return points[index];
    }

    protected override void OnUpgrade(int upgrade) {
        LevelTaskController.I.UpdateTask(KeyAction, 1);
        AwardHandler.I.UpdateAward("award_builder", 1);

        GameManager.I.AddScores(transform.position, upgrade * 50);

        StartCoroutine(IECheckFreeNode());

        OnSuccessUpgrade.Invoke(upgrade);
    }

    protected void SetUpgrade(int value) {
        StartCoroutine(IECheckFreeNode());

        OnSuccessUpgrade.Invoke(value);
    }
}
