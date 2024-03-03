using System.Collections;
using HGL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelGameAward : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI desc;
    [SerializeField] private Image icon;

    private void OnEnable() {
        GameManager.I.IsFinishPanel = true;
    }

    public void Init(string title, string desc, Sprite icon) {
        this.title.text = title;
        this.desc.text = desc;
        this.icon.sprite = icon;
    }

    public void Ok() {
        HGL_WindowManager.I.CloseWindow(null, null, "PanelGameAward", false);
    }

    public IEnumerator IEActive() {
        while (!gameObject.activeSelf) {
            yield return null;
        }
        while (gameObject.activeSelf) {
            yield return null;
        }
    }
}
