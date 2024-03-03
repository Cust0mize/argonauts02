using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelHint : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI text;

    private const float BASE_WIDTH = 300F;

    public void Init(string text) {
        this.text.text = text;
        GetComponent<RectTransform>().sizeDelta = new Vector2(BASE_WIDTH + this.text.preferredWidth, GetComponent<RectTransform>().sizeDelta.y);
    }
}
