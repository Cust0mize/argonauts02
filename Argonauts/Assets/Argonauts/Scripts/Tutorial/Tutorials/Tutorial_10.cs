using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_10 : BaseTutorial {
    public override void Launch() {
        base.Launch();

        BaseInObject bridge = GameManager.I.GetObjectOnNamePart("bridge04");
        if (bridge == null) throw new NullReferenceException("bridge04");

        bridge.OnDestroyed += Bridge_OnDestroyed;
    }

    private void Bridge_OnDestroyed(Node obj) {
        MoveArrow(obj.Position, ArrowSide.RightBot);
        ShowHint(LocalizationManager.GetLocalizedString("hint_10_1"), () => {
            StopTutorial();
        }, true, 5f, -860);
    }
}
