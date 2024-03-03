using UnityEngine;
using UnityEngine.UI;

public class LogoImageHelper : MonoBehaviour
{
    private void Awake() {
        GetComponent<Image>().sprite = LocalizationManager.GetSprite("logo");
    }
}
