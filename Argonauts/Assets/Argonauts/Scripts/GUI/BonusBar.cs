using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BonusBar : MonoBehaviour {
	[SerializeField] Image fillImage;
    [SerializeField] Image spendFillImage;
    [SerializeField] RectTransform effectFill;
    [SerializeField] GameObject effectFilled;
    [SerializeField] ParticleSystem effectPuff;

	public event Action<BonusTypes> OnClickEvent = delegate {};

    private void Start () {
        effectFilled.gameObject.SetActive(false);
        GetComponent<Button>().interactable = false;
    }

    public void UpdateFillBar (float value, float maxValue) {
        effectFill.gameObject.SetActive(true);
        spendFillImage.transform.parent.gameObject.SetActive(false);

		fillImage.fillAmount = value / maxValue;
        effectFill.anchoredPosition = new Vector2(effectFill.anchoredPosition.x, fillImage.rectTransform.sizeDelta.y * value / maxValue);
        effectFilled.gameObject.SetActive(value >= maxValue); 

        GetComponent<Button>().interactable = value >= maxValue;
	}

    public void UpdateSpendBar (float value, float maxValue) {
        effectFill.gameObject.SetActive(false);
        spendFillImage.transform.parent.gameObject.SetActive(true);

        spendFillImage.fillAmount = value / maxValue;
        fillImage.fillAmount = 0f;
        effectFilled.gameObject.SetActive(false);

        GetComponent<Button>().interactable = false;
    }

	public void OnClick () {
		//todo: send event to gamemanager
	}

    public void Ping() {
        StopAllCoroutines();
        StartCoroutine(IEPIng());
    }

    public void StopAnimation() {
        StopAllCoroutines();
        GetComponent<TweenScale>().enabled = false;
        GetComponent<TweenScale>().ResetToBeginning();
        transform.localScale = Vector3.one;
    }

    public void PlayPuffEffect() {
        effectPuff.Stop();
        effectPuff.Play();
    }

    private IEnumerator IEPIng() {
        while(true) {
            GetComponent<TweenScale>().PlayForward();
            yield return new WaitForSeconds(0.6F);
            GetComponent<TweenScale>().enabled = false;
            GetComponent<TweenScale>().ResetToBeginning();

            transform.localScale = Vector3.one;

            yield return new WaitForSeconds(3.0f);
        }
    }
}

public enum BonusTypes {
	Resource,
	Speed,
	Time,
	Work
}
