using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour {
	[SerializeField] private List<GameObject> clouds;
	[SerializeField] private float moveSpeed;
	[SerializeField] private float maxX;
	[SerializeField] private float startX;

	private void Update() {
		for (int i = 0; i < clouds.Count; i++) {
			clouds[i].GetComponent<RectTransform>().anchoredPosition += Vector2.right * moveSpeed * Time.unscaledDeltaTime;
			if (clouds[i].GetComponent<RectTransform>().anchoredPosition.x >= maxX) {
				clouds[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(
					startX - clouds[i].GetComponent<RectTransform>().rect.width,
					clouds[i].GetComponent<RectTransform>().anchoredPosition.y
				);
				clouds[i].GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
			}
		}
	}
}
