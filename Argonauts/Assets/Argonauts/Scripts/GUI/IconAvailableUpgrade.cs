using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconAvailableUpgrade : MonoBehaviour {
    [SerializeField] Sprite spriteAvailable;
    [SerializeField] Sprite spriteUnavailable;
    [SerializeField] Sprite spriteBuildAvailable;
    [SerializeField] Sprite spriteBuildUnavailable;

    private SpriteRenderer spriteRenderer;

    private SpriteRenderer SpriteRenderer {
        get {
            if (spriteRenderer == null) {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            return spriteRenderer;
        }
    }

    private bool isAvailable;

    public bool IsAvailable {
        get {
            return isAvailable;
        }
        set {
            isAvailable = value;
            SpriteRenderer.sprite =  LevelUpgrade == 0 ? (value ? spriteBuildAvailable : spriteBuildUnavailable) : (value ? spriteAvailable : spriteUnavailable);
        }
    }

    private int levelUpgrade;
    public int LevelUpgrade {
        get {
            return levelUpgrade;
        }
        set {
            levelUpgrade = value;
        }
    }
}
