using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AwardScreenManager : LocalSingletonBehaviour<AwardScreenManager> {
    [SerializeField] private List<AwardUI> awards;
    [SerializeField] private UITweener backButton;
    [SerializeField] private Sprite disabledAwardSprite;
    [SerializeField] private PanelAward panelAward;

    public static bool IsOpenedFromMap = false;

    public Sprite DisabledAwardSprite {
        get {
            return disabledAwardSprite;
        }
    }

    private IEnumerator Start() {
        FadeManager.I.ForceFadeIn();

        foreach (AwardUI award in awards) {
            if (award.Key.Equals("award_collector")) {
                award.Init(AwardHandler.I.AllAwardsUnlocked());
            } else {
                award.Init(AwardHandler.I.IsUnlocked(award.Key, award.Level));
            }
        }

        yield return new WaitForSeconds(0.1f);
        yield return FadeManager.I.FadeOut();

        backButton.GetComponent<UITweener>().PlayForward();
    }

    public void OnButtonBackClick() {
        if (IsOpenedFromMap) {
            TransitionManager.I.LoadMap();
        } else {
            TransitionManager.I.LoadMenu();
        }
    }

    public void Show(AwardUI award) {
        int target = 0;
        if (!award.Key.Equals("award_collector")) {
            target = AwardTargets.GetTargets(award.Key)[award.Level - 1];
        }

        panelAward.gameObject.SetActive(true);
        panelAward.Init(LocalizationManager.GetLocalizedString(string.Format("{0}_{1}_name", award.Key, award.Level)),
                        string.Format(LocalizationManager.GetLocalizedString(string.Format("{0}_{1}_desc", award.Key, award.Level)), target), award.GetComponent<RectTransform>().position);
    }

    public void Hide() {
        panelAward.gameObject.SetActive(false);
    }
}
