using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_03 : BaseTutorial {
    private BaseInObject cachedTent;

    public override void Launch() {
        base.Launch();

        BaseInObject tent = GameManager.I.CampObject;
        if (tent == null) throw new NullReferenceException("tent");
        tent.OnBecameAvailable += Tent_OnBecameAvailable;
    }

    private void Tent_OnBecameAvailable(BaseInObject tent) {
        tent.OnBecameAvailable -= Tent_OnBecameAvailable;
        cachedTent = tent;

        MoveArrow(tent.transform.position, ArrowSide.LeftBot);
        ShowHint(LocalizationManager.GetLocalizedString("hint_2_4"), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 550F), null, false, 1f);

        OverseerController.I.OnObjectAddedInOrder += OnTentAddedInOrder;
    }

    private void OnTentAddedInOrder(BaseInObject obj) {
        if (obj == cachedTent) {
            StopTutorial();
        }
    }
}
