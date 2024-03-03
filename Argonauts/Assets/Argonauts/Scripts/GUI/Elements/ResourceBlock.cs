using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class ResourceBlock : MonoBehaviour {
    public TextMeshProUGUI Text;
    public Image RedImage;

    public void PlayScale() {
        Text.GetComponent<Scaller>().Play();
    }

    public void Ping() {
        RedImage.GetComponent<RedFade>().StartPing();
    }
}
