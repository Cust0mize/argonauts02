using TMPro;
using UnityEngine;

public class PanelBonusDescription : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public GameBonusElement CurrentElement { get; set; }

    public void SetText(string text)
    {
        this.text.text = text;
    }
}
