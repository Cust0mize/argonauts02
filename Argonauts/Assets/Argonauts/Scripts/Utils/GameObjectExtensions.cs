using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions {
    public static void Hide(this GameObject gameObject, bool withChilds = true) {
        SpriteRenderer[] sp = gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer s in sp) {
            s.enabled = false;
        }
        SpriteRenderer currentS = gameObject.GetComponent<SpriteRenderer>();
        currentS.enabled = false;
    }

    public static void Show(this GameObject gameObject, bool withChilds = true) {
        SpriteRenderer[] sp = gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer s in sp) {
            s.enabled = true;
        }
        SpriteRenderer currentS = gameObject.GetComponent<SpriteRenderer>();
        currentS.enabled = true;
    }
}