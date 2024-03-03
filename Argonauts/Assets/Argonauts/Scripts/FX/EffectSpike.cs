using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpike : MonoBehaviour {
    [SerializeField] private Color effectColor1;
    [SerializeField] private Color effectColor2;

    private void Awake () {
        ApplyColors(GetComponent<ParticleSystem>());
        ApplyColors(transform.GetChild(0).GetComponent<ParticleSystem>());
    }

    private void ApplyColors(ParticleSystem particleSystem){
        ParticleSystem.MainModule main = particleSystem.main;

        ParticleSystem.MinMaxGradient minMaxGradient = main.startColor;
        minMaxGradient.mode = ParticleSystemGradientMode.RandomColor;

        Gradient gr = new Gradient();
        gr.mode = GradientMode.Blend;
        gr.SetKeys(new GradientColorKey[] { new GradientColorKey(effectColor1, 0f), new GradientColorKey(effectColor2, 1f) },
                   new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 0f) }
                  );

        minMaxGradient.gradient = gr;

        main.startColor = minMaxGradient;
    }
}
