using TMPro;
using UnityEngine;

public class ObjectFlag : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI queueNumberText;

    public void SetNumber(int number) {
        queueNumberText.text = number.ToString();
    }

    public void SetActive(bool value) {
        gameObject.SetActive(value);
    }
}
