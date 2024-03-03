using System.Collections.Generic;
using System.Linq;

public static class AwardTargets {
    private static Dictionary<string, int[]> Targets = new Dictionary<string, int[]>() {
        { "award_builder", new int[] { 10,50,100 } },
        { "award_food", new int[] { 5,500,1000 } },
        { "award_gold", new int[] { 30,500,1000 } },
        { "award_obstacles", new int[] { 5,200,500 } },
        { "award_reader", new int[] { ProjectSettings.I.isAllLevelsEnabled ? 7 : 6 } },
        { "award_stars", new int[] { 5,25, ProjectSettings.I.isAllLevelsEnabled ? 60 : 50 } },
        { "award_stone", new int[] { 20,500,1000 } },
        { "award_trader", new int[] { 5,150,250 } },
        { "award_victory", new int[] { 5,25,50 } },
        { "award_wood", new int[] { 30,500,1000 } }
    };

    public static int[] GetTargets(string key) {
        if (Targets.ContainsKey(key)) {
            return Targets[key];
        }
        throw new System.ArgumentNullException("unknow target, key: " + key);
    }

    public static string[] GetTargetKeys() {
        return Targets.Keys.ToArray();
    }
}
