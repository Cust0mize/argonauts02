using UnityEngine;

public class Tutorial_04 : BaseTutorial {
    public override void Launch() {
        base.Launch();

        GameManager.I.OnBonusCameAvailable += OnBonusCameAvailable;
    }

    private void OnBonusCameAvailable(string bonusType) {
        GameManager.I.OnBonusCameAvailable -= OnBonusCameAvailable;
        GameManager.I.OnBonusCameActive += OnBonusCameActive;

        MoveArrowUI(GameGUIManager.I.SpeedBar.transform.parent.GetComponent<RectTransform>().anchoredPosition, new Vector2(0.5F, 0f), new Vector2(0.5f, 0f), ArrowSide.Top, 120);
        ShowHint(LocalizationManager.GetLocalizedString("hint_4_1"), new Vector2(0.5F, 0), new Vector2(0.5F, 0f), 330, null);
    }

    private void OnBonusCameActive(string bonusType) {
        GameManager.I.OnBonusCameActive -= OnBonusCameActive;
        StopTutorial();
    }
}
