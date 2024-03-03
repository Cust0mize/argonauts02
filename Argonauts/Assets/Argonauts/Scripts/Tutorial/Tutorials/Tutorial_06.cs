using System;
using UnityEngine;

public class Tutorial_06 : BaseTutorial {
    public override void Launch() {
        base.Launch();

        BaseInObject mp = GameManager.I.GetObjectOnNamePart("market_place");
        if (mp == null) throw new NullReferenceException("market_place02");

        MoveArrow(mp.transform.position, ArrowSide.Bot);
        ShowHint(LocalizationManager.GetLocalizedString("hint_6_1"), new Vector2(0.5F, 0.5F), new Vector2(0.5F, 0.5F), -50F, StopTutorial, true, 5F);
    }
}
