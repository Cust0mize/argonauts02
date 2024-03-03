using UnityEngine;

public class Tutorial_11 : BaseTutorial {
    public override void Launch() {
        base.Launch();
        ShowWorkBonusHint();
    }

    private void ShowWorkBonusHint() {
        MoveArrowUI(GameGUIManager.I.WorkBar.transform.parent.GetComponent<TweenAnchoredPosition>().to + Vector3.right * 75, new Vector2(0.5F, 0f), new Vector2(0.5f, 0f), ArrowSide.Top, 120);
        ShowHint(LocalizationManager.GetLocalizedString("hint_11_1"), new Vector2(0.5F, 0), new Vector2(0.5F, 0f), 330, StopTutorial, true, 5F);
    }
}
