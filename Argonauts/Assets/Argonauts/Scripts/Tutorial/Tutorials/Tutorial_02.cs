using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_02 : BaseTutorial {
    private BuildObject availableBuild;

    private bool farmUpgraded;
    private bool goldmineUpgraded;
    private bool quarryUpgraded;
    private bool sawmillUpgraded;
    private bool tentUpgraded;

    public override void Launch() {
        base.Launch();

        BaseInObject bush = GameManager.I.GetObjectOnNamePart("bush");
        if (bush == null) throw new NullReferenceException("bush");

        OverseerController.I.OnObjectAddedInOrder += Bush_AddedInOrder;

        MoveArrow(bush.transform.position, ArrowSide.RightBot);
        ShowHint(LocalizationManager.GetLocalizedString("hint_2_1"), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 590F), null, false, 1f);
    }

    private void Bush_AddedInOrder(BaseInObject obj) {
        OverseerController.I.OnObjectAddedInOrder -= Bush_AddedInOrder;

        HideHint();
        HideLastArrow();

        OverseerController.I.OnArriveHomeEvent += OnArriveHomeBush;
    }

    private void OnArriveHomeBush(BaseInObject obj) {
        OverseerController.I.OnArriveHomeEvent -= OnArriveHomeBush;

        BaseInObject goldmine = GameManager.I.GetObjectOnNamePart("goldmine");
        BaseInObject sawmill = GameManager.I.GetObjectOnNamePart("sawmill");

        if (goldmine == null) throw new NullReferenceException("goldmine");
        if (sawmill == null) throw new NullReferenceException("sawmill");

        goldmine.OnBecameAvailable += Goldmine_OnBecameAvailable;
        sawmill.OnBecameAvailable += Sawmill_OnBecameAvailable;
    }

    private void Goldmine_OnBecameAvailable(BaseInObject goldmine) {
        goldmine.OnBecameAvailable -= Goldmine_OnBecameAvailable;

        (goldmine as BuildObject).OnSuccessUpgrade += Goldmine_OnSuccessUpgrade;
        StartCoroutine(GoldmineHandler(goldmine));
    }

    private void Sawmill_OnBecameAvailable(BaseInObject sawmill) {
        sawmill.OnBecameAvailable -= Sawmill_OnBecameAvailable;

        (sawmill as BuildObject).OnSuccessUpgrade += Sawmill_OnSuccessUpgrade;
        StartCoroutine(SawmillHandler(sawmill));
    }

    private IEnumerator GoldmineHandler(BaseInObject goldmine) {
        while (availableBuild != null) yield return new WaitForSeconds(0.1f);
        if (goldmineUpgraded) yield break;
        yield return new WaitForSeconds(0.1f);

        if (availableBuild != null) {
            StartCoroutine(GoldmineHandler(goldmine));
        } else {
            availableBuild = goldmine as BuildObject;
            OverseerController.I.OnObjectAddedInOrder += Goldmine_AddedInOrder;

            MoveArrow(goldmine.transform.position, ArrowSide.RightTop);
            ShowHint(LocalizationManager.GetLocalizedString("hint_2_6"), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 330F), null, false, 1f);
        }
    }

    private IEnumerator SawmillHandler(BaseInObject sawmill) {
        while (availableBuild != null) yield return new WaitForSeconds(0.1f);
        if (sawmillUpgraded) yield break;
        yield return new WaitForSeconds(0.1f);

        if (availableBuild != null) {
            StartCoroutine(SawmillHandler(sawmill));
        } else {
            availableBuild = sawmill as BuildObject;
            OverseerController.I.OnObjectAddedInOrder += Sawmill_AddedInOrder;

            MoveArrow(sawmill.transform.position, ArrowSide.RightBot);
            ShowHint(LocalizationManager.GetLocalizedString("hint_2_3"), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 610F), null, false, 1f);
        }
    }

    private void Goldmine_AddedInOrder(BaseInObject goldmine) {
        if ((availableBuild as BaseInObject) == goldmine) {
            OverseerController.I.OnObjectAddedInOrder -= Goldmine_AddedInOrder;

            HideHint();
            HideLastArrow();
            availableBuild = null;
        }
    }

    private void Sawmill_AddedInOrder(BaseInObject sawmill) {
        if ((availableBuild as BaseInObject) == sawmill) {
            OverseerController.I.OnObjectAddedInOrder -= Sawmill_AddedInOrder;

            HideHint();
            HideLastArrow();
            availableBuild = null;
        }
    }

    private void Goldmine_OnSuccessUpgrade(int value) {
        goldmineUpgraded = true;
    }

    private void Sawmill_OnSuccessUpgrade(int value) {
        sawmillUpgraded = true;
    }
}
