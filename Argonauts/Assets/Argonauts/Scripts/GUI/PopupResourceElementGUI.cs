using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupResourceElementGUI : MonoBehaviour {
    [SerializeField]
    Image iconResources;
    [SerializeField]
    TextMeshProUGUI countText;

    [SerializeField] Color increaseColor = Color.green;
    [SerializeField] Color decreaseColor = Color.red;

    private const float amplitude = 1f;
    private const float frequency = 3f;
    private const float offset = 0.9f;

    public void Init (Resource resource) {
        iconResources.sprite = ResourceManager.I.GetResourceSprite(string.Format("{0}01", resource.Type.ToString()));

        if (iconResources.sprite == null)
            iconResources.sprite = ResourceManager.I.GetResourceSprite(string.Format("{0}", resource.Type.ToString()));

        countText.text = string.Format("{0}{1}", resource.Count > 0 ? "+" : string.Empty, resource.Count);
        countText.transform.GetComponent<TextMeshProUGUI>().color = resource.Count > 0 ? increaseColor : decreaseColor;

        if (iconResources.sprite == null) DestroyImmediate(gameObject);
    }

    public void DoPopUp (float lifetime, float speed) {
        StopAllCoroutines();
        StartCoroutine(IEDoPopUp(lifetime, speed));
    }

    private IEnumerator IEDoPopUp (float lifetime, float speed) {
        transform.position += Vector3.up * 1f;

        float startLifetime = lifetime;

        while (lifetime > 0) {
            transform.position += Vector3.up * Time.deltaTime * speed;
            transform.localScale = Vector3.one * GetSinusoid((startLifetime - lifetime) / (startLifetime * 2F));
            lifetime -= Time.deltaTime;
            yield return null;
        }
        DestroyImmediate(gameObject);
    }

    private float GetSinusoid(float t) {
        return amplitude * Mathf.Sin(frequency * t + offset);
    }
}
