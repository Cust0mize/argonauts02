using UnityEngine;

public static partial class AnimatorExtensions {
    public static bool HasParameterOfType (this Animator self, string name, AnimatorControllerParameterType type) {
        var parameters = self.parameters;
        foreach (var currParam in parameters) {
            if (currParam.type == type && currParam.name == name) {
                return true;
            }
        }
        return false;
    }

    public static bool HasBool (this Animator self, string name) {
        var parameters = self.parameters;
        foreach (var currParam in parameters) {
            if (currParam.type == AnimatorControllerParameterType.Bool && currParam.name == name) {
                return true;
            }
        }
        return false;
    }
}