using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_17  : BaseTutorial {
    public override void Launch() {
        base.Launch();
        ShowResourceBonusHint();
    }

    private void ShowResourceBonusHint() {
        MoveArrowUI(GameGUIManager.I.ResourceBar.transform.parent.GetComponent<TweenAnchoredPosition>().to + Vector3.right * 65F, new Vector2(0.5F, 0f), new Vector2(0.5f, 0f), ArrowSide.Top, 120);
        ShowHint(LocalizationManager.GetLocalizedString("hint_17_1"), new Vector2(0.5F, 0), new Vector2(0.5F, 0f), 330, StopTutorial, true, 5F);
    }
}
