using System.Collections;
using System.Collections.Generic;
using SpriteParticleEmitter;
using TMPro;
using UnityEngine;

public class PanelScore : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI scoreText;

    public void UpdateScores(int newScore) {
        scoreText.text = newScore.ToString();
        scoreText.GetComponent<Scaller>().Play();
    }
}
