using System;
using UnityEngine;

public class Tutorial_12 : BaseTutorial {
    BaseInObject altar;

    public override void Launch() {
        base.Launch();

        altar = GameManager.I.GetNearestObject(GameManager.I.CampObject.transform.position, "altar");
        if (altar == null) throw new NullReferenceException("altar");

        altar.OnBecameAvailable += Altar_OnBecameAvailable;
    }

    private void Altar_OnBecameAvailable(BaseInObject altar) {
        altar.OnBecameAvailable -= Altar_OnBecameAvailable;

        MoveArrow(altar.Node.Position, ArrowSide.RightTop);
        ShowHint(LocalizationManager.GetLocalizedString("hint_12_1"), () => {
            OverseerController.I.OnObjectAddedInOrder -= OnAltarAddedInOrder;

            StopTutorial();
        }, true, 5f, -860);

        OverseerController.I.OnObjectAddedInOrder += OnAltarAddedInOrder;
    }

    private void OnAltarAddedInOrder(BaseInObject altar) {
        if (this.altar == altar) {
            OverseerController.I.OnObjectAddedInOrder -= OnAltarAddedInOrder;
            StopTutorial();
        }
    }
}
