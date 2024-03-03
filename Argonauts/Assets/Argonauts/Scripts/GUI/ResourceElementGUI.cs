using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceElementGUI : MonoBehaviour {
    [SerializeField]
    Image iconResources;
    [SerializeField]
    TextMeshProUGUI countText;

    public void Init (Resource resource, bool checkEnought = false, bool enought = false) {
        iconResources.sprite = ResourceManager.I.GetResourceSprite(string.Format("{0}03", resource.Type.ToString()));

        if (iconResources.sprite == null)
            iconResources.sprite = ResourceManager.I.GetResourceSprite(string.Format("{0}", resource.Type.ToString()));

        if(resource.Type == Resource.Types.Gem) {
            iconResources.sprite = ResourceManager.I.GetResourceSprite(string.Format("{0}01", resource.Type.ToString()));
        }

        countText.text = resource.Count.ToString();
        if (checkEnought) {
            if (enought) {
                countText.color = new Color32(0, 0, 80, 255);
            } else {
                countText.color = new Color32(237, 28, 36, 255);
            }
        }
        if (iconResources.sprite == null) DestroyImmediate(gameObject);
    }
}
