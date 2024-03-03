using System.Collections;
using System.Collections.Generic;
using HGL;
using UnityEngine;
using UnityEngine.Analytics;

public class AwardShower : LocalSingletonBehaviour<AwardShower> {
    private List<AwardUnlockTarget> unlockedAwards = new List<AwardUnlockTarget>();

    public override void DoAwake() {
        AwardHandler.I.OnAwardUnlocked += OnAwardUnlocked;
    }

    private void OnAwardUnlocked(string key, int level, int target) {
        AnalyticsEvent.Custom("achievement_unlocked", new Dictionary<string, object> {
            { "achievement_name", key },
            { "achievement_level", level }
        });
        unlockedAwards.Add(new AwardUnlockTarget(key, level, target));
    }

    public IEnumerator DoShow() {
        GameObject panel = HGL_WindowManager.I.GetWindow("PanelGameAward").gameObject;

        while (unlockedAwards.Count > 0) {
            panel.GetComponent<PanelGameAward>().Init(LocalizationManager.GetLocalizedString(string.Format("{0}_{1}_name", unlockedAwards[0].Key, unlockedAwards[0].Level)),
                                                      string.Format(LocalizationManager.GetLocalizedString(string.Format("{0}_{1}_desc", unlockedAwards[0].Key, unlockedAwards[0].Level)), unlockedAwards[0].Target),
                                                      ResourceManager.I.GetAwardIcon(string.Format("{0}_{1}", unlockedAwards[0].Key, unlockedAwards[0].Level)));
            HGL_WindowManager.I.OpenWindow(null, null, "PanelGameAward", false, false);
            yield return panel.GetComponent<PanelGameAward>().IEActive();
            unlockedAwards.RemoveAt(0);
                                                      
        }
        yield return 0;
    }

    public override void DoDestroy() {
        AwardHandler.I.OnAwardUnlocked -= OnAwardUnlocked;
    }
}

public class AwardUnlockTarget {
    public string Key;
    public int Level;
    public int Target;

    public AwardUnlockTarget(string key, int level, int target) {
        Key = key;
        Level = level;
        Target = target;
    }
}
