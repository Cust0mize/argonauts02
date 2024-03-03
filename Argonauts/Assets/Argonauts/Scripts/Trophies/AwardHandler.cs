using System;
using System.Collections.Generic;
using UnityEngine;

public class AwardHandler : GlobalSingletonBehaviour<AwardHandler> {
    public event Action<string, int, int> OnAwardUnlocked = delegate { };

    private Dictionary<string, Award> cachedAward = new Dictionary<string, Award>();

    public void UpdateAward(string key, int increaseValue) {
        int[] targets = AwardTargets.GetTargets(key);
        bool[] previousUnlocked = new bool[targets.Length];

        Award award = GetAward(key);

        for (int i = 0; i < targets.Length; i++) {
            if (award.Value >= targets[i]) previousUnlocked[i] = true;
        }

        award.Value += increaseValue;

        for (int i = 0; i < targets.Length; i++) {
            if (award.Value >= targets[i] && !previousUnlocked[i]) {
                //Debug.Log("Collector award, key: " + key + " , value: " + award.Value + " , target: " + targets[i]);

                OnAwardUnlocked.Invoke(key, i + 1, targets[i]);
            }
        }
    }

    private Award GetAward(string key) {
        if (!cachedAward.ContainsKey(key)) {
            cachedAward.Add(key, UserData.I.GetAward(key));
        }
        return cachedAward[key];
    }

    //private void Update() {
    //    if (Input.GetKeyDown(KeyCode.D)) {
    //        foreach (string k in AwardTargets.GetTargetKeys()) {
    //            Award award = GetAward(k);
    //            Debug.Log(award.Key + " - " + award.Value);
    //        }
    //    }

    //    if (Input.GetKeyDown(KeyCode.B)) {
    //        foreach (string s in AwardTargets.GetTargetKeys()) {
    //            foreach (int t in AwardTargets.GetTargets(s)) {
    //                Award award = GetAward(s);
    //                int val = t - award.Value;
    //                UpdateAward(s, val);
    //            }
    //        }
    //    }
    //}

    public bool IsUnlocked(string key, int level) {
        Award award = UserData.I.GetAward(key);
        int target = AwardTargets.GetTargets(key)[level - 1];
        return award.Value >= target;
    }

    public void SaveAwards() {
        foreach (string k in cachedAward.Keys) {
            UserData.I.SaveAward(cachedAward[k]);
        }
    }

    public bool AllAwardsUnlocked() {
        foreach (string s in AwardTargets.GetTargetKeys()) {
            Award award = UserData.I.GetAward(s);
            int[] targets = AwardTargets.GetTargets(s);

            if (award.Value < targets[targets.Length - 1]) return false;
        }
        return true;
    }

    public void ClearCache() {
        AwardHandler.I.SaveAwards();
        cachedAward.Clear();
    }

    public override void DoDestroy() {
        AwardHandler.I.SaveAwards();
    }
}
