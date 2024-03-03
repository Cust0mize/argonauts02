using UnityEngine;
using TMPro;

public class LocalizationItem : MonoBehaviour
{

    public string stringId;

    void Awake() {
        Refresh();
        LocalizationManager.OnLanguageLoaded += Refresh;
    }

    public void Refresh() {
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();

        if (text != null) {
            text.text = LocalizationManager.GetLocalizedString(stringId);
        }
    }

    void OnDestroy() {
        LocalizationManager.OnLanguageLoaded -= Refresh;
    }
}
