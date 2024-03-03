using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelAward : MonoBehaviour {
    private const float OFFSET_TOP = 50F;
    private const float OFFSET_RIGHT = 50F;
    private const float OFFSET_BOT = -50F;
    private const float OFFSET_LEFT = -50F;

    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI desc;

    public void Init(string title, string desc, Vector2 position) {
        this.title.text = title;
        this.desc.text = desc;

        GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);

        position.y += OFFSET_TOP;

        bool topDecreased = false;

        if (position.x >= Screen.width * 0.8F) {
            topDecreased = true;
            position.y -= OFFSET_TOP;
            position.x += OFFSET_LEFT;
            GetComponent<RectTransform>().pivot = new Vector2(1f, 0.5f);
        } else if (position.x <= Screen.width * 0.2F) {
            topDecreased = true;
            position.y -= OFFSET_TOP;
            position.x += OFFSET_RIGHT;
            GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
        }

        if (position.y >= Screen.height * 0.85F) {
            if (!topDecreased)
                position.y -= OFFSET_TOP;
            position.y += OFFSET_BOT;
            GetComponent<RectTransform>().pivot = new Vector2(GetComponent<RectTransform>().pivot.x, 1f);
        } else if (position.y <= Screen.height * 0.15f) {
            if (topDecreased)
                position.y += OFFSET_TOP;
            GetComponent<RectTransform>().pivot = new Vector2(GetComponent<RectTransform>().pivot.x, 0F);
        }

        GetComponent<RectTransform>().position = position;
    }
}
