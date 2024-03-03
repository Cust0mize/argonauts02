//----------------------------------------------
//            NGUI: Next-Gen UI kit Extension
// Copyright © 2017 Car3man
//----------------------------------------------

using UnityEngine;

[AddComponentMenu("NGUI_Extension/Tween/Tween Alpha")]
public class TweenColor : UITweener {
    public Color from;
    public Color to;
    public bool updateTable = false;

    SpriteRenderer mSpriteRenderer;

    public SpriteRenderer cachedSpriteRenderer { get { if(mSpriteRenderer == null) mSpriteRenderer = GetComponent<SpriteRenderer>(); return mSpriteRenderer; } }

    private float _value = 0;
    public float value { get { return _value;  } set { if(cachedSpriteRenderer != null) cachedSpriteRenderer.color = Color.Lerp(from, to, value); _value = value; } }

    public Color Color {
        get {
            return Color.Lerp(from, to, value);
        }
    }

    public Color GetColor(float value) {
        return Color.Lerp(from, to, value);
    }

    [System.Obsolete("Use 'value' instead")]
    public float colorLerp { get { return this.value; } set { this.value = value; } }

    /// <summary>
    /// Tween the value.
    /// </summary>

    protected override void OnUpdate(float factor, bool isFinished) {
        value = factor;
    }

    /// <summary>
    /// Start the tweening operation.
    /// </summary>

    static public TweenColor Begin(GameObject go, float duration, float colorLerp) {
        TweenColor comp = UITweener.Begin<TweenColor>(go, duration);
        comp.from = comp.Color;
        comp.to = comp.GetColor(colorLerp);

        if(duration <= 0f) {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }
}
