using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskBlock : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image icon;

    public void SetText(string value) {
        text.text = value;
    }

    public void SetIcon(Sprite value) {
        icon.sprite = value;
    }
}
